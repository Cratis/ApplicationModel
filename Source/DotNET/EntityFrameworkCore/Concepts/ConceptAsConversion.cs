// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cratis.Applications.EntityFrameworkCore.Concepts;

/// <summary>
/// Provides extension methods for applying concept-based value conversions.
/// </summary>
public static class ConceptAsConversion
{
    /// <summary>
    /// Applies value conversion for all properties that are of <see cref="ConceptAs{T}"/> type  in the specified <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder to apply the concept-based conversion to.</param>
    /// <param name="databaseType">The database provider, if specific configuration is needed.</param>
    public static void ApplyConceptAsConversion(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsOwned() || !entityType.HasConceptProperties()) continue;

            var entityTypeBuilder = modelBuilder.Entity(entityType.Name);
            entityTypeBuilder.ApplyConceptAsConversion(databaseType);
        }
    }

    /// <summary>
    /// Checks if the entity has any properties of <see cref="ConceptAs{T}"/> type.
    /// </summary>
    /// <param name="entity">The entity type to check for concept properties.</param>
    /// <returns>True if the entity has concept properties; otherwise, false.</returns>
    public static bool HasConceptProperties(this IMutableEntityType entity) =>
        entity.ClrType.GetProperties()
            .Any(p => p.PropertyType.IsConcept());

    /// <summary>
    /// Applies value conversion for all properties that are of <see cref="ConceptAs{T}"/> type in the specified <see cref="EntityTypeBuilder"/>.
    /// </summary>
    /// <param name="entityTypeBuilder">The entity type builder to apply the concept-based conversion to.</param>
    /// <param name="databaseType">The database provider, if specific configuration is needed.</param>
    public static void ApplyConceptAsConversion(this EntityTypeBuilder entityTypeBuilder, DatabaseType databaseType)
    {
        var properties = entityTypeBuilder.Metadata.ClrType.GetProperties()
            .Where(p => p.PropertyType.IsConcept())
            .ToList();

        foreach (var property in properties)
        {
            var propertyBuilder = entityTypeBuilder.Property(property.Name);
            propertyBuilder.AsConcept(databaseType);
        }
    }
}
