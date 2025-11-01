// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Concepts;
using Cratis.Applications.EntityFrameworkCore.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
            .Where(et => et.IsOwned() || dbSetTypes.Contains(et.ClrType))
            .ToArray();

        var databaseType = Database.GetDatabaseType();
        modelBuilder.ApplyJsonConversion(entityTypesForConverters, databaseType);
        modelBuilder.ApplyConceptAsConversion(entityTypesForConverters, databaseType);
        modelBuilder.ApplyGuidConversion(entityTypesForConverters, databaseType);
        base.OnModelCreating(modelBuilder);
    }
}
