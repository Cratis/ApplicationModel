// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Concepts;
using Cratis.Applications.EntityFrameworkCore.Json;
using Cratis.Applications.EntityFrameworkCore.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Base class for DbContexts that needs standard things configured.
/// </summary>
/// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
public class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        this.GetService<IEntityTypeRegistrar>().RegisterEntityMaps(this, modelBuilder);

        var entityTypes = modelBuilder.Model.GetEntityTypes();
        var dbSetTypes = GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .ToArray();

        var entityTypesForConverters = entityTypes
            .Where(IsRelevantForConverters(dbSetTypes))
            .ToArray();

        var databaseType = Database.GetDatabaseType();
        modelBuilder.ApplyJsonConversion(entityTypesForConverters, databaseType);
        modelBuilder.ApplyConceptAsConversion(entityTypesForConverters, databaseType);
        modelBuilder.ApplyGuidConversion(entityTypesForConverters, databaseType);
        base.OnModelCreating(modelBuilder);
    }

    Func<IMutableEntityType, bool> IsRelevantForConverters(Type[] dbSetTypes) => et =>
        et.IsOwned() ||
        dbSetTypes.Contains(et.ClrType) ||
        dbSetTypes.Any(dbSetType =>
            dbSetType.GetProperties().Any(p =>
                p.PropertyType == et.ClrType ||
                (p.PropertyType.IsGenericType &&
                 p.PropertyType.GetGenericArguments().Contains(et.ClrType))));
}
