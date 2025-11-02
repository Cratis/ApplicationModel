// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Cratis.Types;
using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Mapping;

/// <summary>
/// Represents an implementation of <see cref="IEntityTypeRegistrar"/>.
/// </summary>
/// <param name="types"><see cref="ITypes"/> for type discovery.</param>
/// <param name="serviceProvider"><see cref="IServiceProvider"/> for resolving dependencies.</param>
[Singleton]
public class EntityTypeRegistrar(ITypes types, IServiceProvider serviceProvider) : IEntityTypeRegistrar
{
    readonly Dictionary<Type, Type> _entityMaps = types
        .FindMultiple(typeof(IEntityTypeConfiguration<>))
        .ToDictionary(
            t => t.GetInterface(typeof(IEntityTypeConfiguration<>).FullName!)!.GetGenericArguments()[0],
            t => t);

    /// <inheritdoc/>
    public void RegisterEntityMaps(DbContext dbContext, ModelBuilder modelBuilder)
    {
        var dbContextType = dbContext.GetType();
        var dbSetProperties = dbContextType.GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                       p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var property in dbSetProperties)
        {
            var entityType = property.PropertyType.GetGenericArguments()[0];
            if (_entityMaps.TryGetValue(entityType, out var mapType))
            {
                var configureMethod = typeof(EntityMapConfigurer<>)
                    .MakeGenericType(entityType)
                    .GetMethod(nameof(EntityMapConfigurer<object>.Configure))!;
                configureMethod.Invoke(null, [mapType, serviceProvider, modelBuilder]);
            }
        }
    }

    static class EntityMapConfigurer<T>
        where T : class
    {
        public static void Configure(Type mapType, IServiceProvider serviceProvider, ModelBuilder modelBuilder)
        {
            var entityMap = (IEntityTypeConfiguration<T>)serviceProvider.GetService(mapType)!;
            var entityTypeBuilder = modelBuilder.Entity<T>();
            entityMap.Configure(entityTypeBuilder);
        }
    }
}