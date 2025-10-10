// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
    /// <param name="database">The database provider, if specific configuration is needed.</param>
    public static void ApplyConceptAsConversion(this ModelBuilder modelBuilder, DatabaseFacade database)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType.IsConcept())
                .ToList();

            foreach (var property in properties)
            {
                var propertyBuilder = modelBuilder.Entity(entityType.Name).Property(property.Name);
                propertyBuilder.AsConcept(database);
            }
        }
    }
}

