// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Json;

/// <summary>
/// Provides JSON conversion capabilities for entity properties.
/// </summary>
public static class JsonConversion
{
    /// <summary>
    /// Applies JSON conversion to all properties marked with the <see cref="JsonAttribute"/> in the specified <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder to apply the JSON conversion to.</param>
    /// <param name="databaseType">The database provider, if specific configuration is needed.</param>
    public static void ApplyJsonConversion(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var entityTypeBuilder = modelBuilder.Entity(entityType.Name);
            entityTypeBuilder.ApplyJsonConversion(databaseType);
        }
    }
}
