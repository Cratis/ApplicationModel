// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Concepts;
using Cratis.Applications.EntityFrameworkCore.Json;
using Microsoft.EntityFrameworkCore;

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
        var entityTypes = modelBuilder.Model.GetEntityTypes();
        var dbSetTypes = entityTypes
            .Select(et => et.ClrType)
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(t => t.GetGenericArguments()[0])
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
