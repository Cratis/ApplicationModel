// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
    public static void ApplyConceptAsConversion(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType.IsConcept())
                .ToList();

            foreach (var property in properties)
            {
                var conceptValueType = property.PropertyType.GetConceptValueType();
                var propertyBuilder = modelBuilder.Entity(entityType.Name).Property(property.Name);
                var converterType = typeof(ConceptAsValueConverter<,>).MakeGenericType(property.PropertyType, conceptValueType);
                var comparerType = typeof(ConceptAsValueComparer<,>).MakeGenericType(property.PropertyType, conceptValueType);
                var converter = Activator.CreateInstance(converterType) as ValueConverter;
                var comparer = Activator.CreateInstance(comparerType) as ValueComparer;

                propertyBuilder.HasConversion(converter);
                propertyBuilder.Metadata.SetValueConverter(converter);
                propertyBuilder.Metadata.SetValueComparer(comparer);
            }
        }
    }

    sealed class ConceptAsValueConverter<TConcept, TPrimitive>() : ValueConverter<TConcept, TPrimitive>(
        v => v.Value,
        v => (TConcept)ConceptFactory.CreateConceptInstance(typeof(TConcept), v))
        where TConcept : notnull, ConceptAs<TPrimitive>
        where TPrimitive : notnull, IComparable;

    sealed class ConceptAsValueComparer<TConcept, TPrimitive>() : ValueComparer<TConcept>(
        (l, r) => l!.Equals(r),
        v => v.GetHashCode())
        where TConcept : notnull, ConceptAs<TPrimitive>
        where TPrimitive : notnull, IComparable;
}