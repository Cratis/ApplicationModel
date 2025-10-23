// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for applying GUID conversions to entity properties.
/// </summary>
public static class GuidConversion
{
    /// <summary>
    /// Applies GUID conversion for all properties of type <see cref="Guid"/> in the specified <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder to apply the GUID conversion to.</param>
    /// <param name="databaseType">The database type, if specific configuration is needed.</param>
    public static void ApplyGuidConversion(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => !t.ClrType.IsConcept()))
        {
            if (entityType.IsOwned()) continue;

            var entityTypeBuilder = modelBuilder.Entity(entityType.Name);
            entityTypeBuilder.ApplyGuidConversion(databaseType);
        }
    }

    /// <summary>
    /// Applies GUID conversion for all properties of type <see cref="Guid"/> in the specified <see cref="EntityTypeBuilder"/>.
    /// </summary>
    /// <param name="entityTypeBuilder">The entity type builder to apply the GUID conversion to.</param>
    /// <param name="databaseType">The database type, if specific configuration is needed.</param>
    public static void ApplyGuidConversion(this EntityTypeBuilder entityTypeBuilder, DatabaseType databaseType)
    {
        var properties = entityTypeBuilder.Metadata.ClrType.GetProperties()
            .Where(p => p.PropertyType == typeof(Guid))
            .ToList();

        foreach (var property in properties)
        {
            var propertyBuilder = entityTypeBuilder.Property(property.Name);
            propertyBuilder.AsGuid(databaseType);
        }
    }
}