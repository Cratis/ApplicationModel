// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Cratis.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cratis.Applications.EntityFrameworkCore.Json;

/// <summary>
/// Provides JSON conversion capabilities for entity properties.
/// </summary>
public static class JsonConversion
{
    static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerOptions.Default)
    {
        WriteIndented = false,
        Converters =
        {
            new ConceptAsJsonConverterFactory()
        }
    };

    /// <summary>
    /// Applies JSON conversion to all properties marked with the <see cref="JsonAttribute"/> in the specified <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder to apply the JSON conversion to.</param>
    /// <param name="entityTypes">The entity types to apply the JSON conversion to.</param>
    /// <param name="databaseType">The database provider, if specific configuration is needed.</param>
    public static void ApplyJsonConversion(this ModelBuilder modelBuilder, IEnumerable<IMutableEntityType> entityTypes, DatabaseType databaseType)
    {
        entityTypes = entityTypes.Where(t => t.HasJsonProperties());
        foreach (var entityType in entityTypes)
        {
            var entityTypeBuilder = modelBuilder.Entity(entityType.Name);
            entityTypeBuilder.ApplyJsonConversion(databaseType);
        }
    }

    /// <summary>
    /// Checks if the entity has any JSON properties.
    /// </summary>
    /// <param name="entity">The entity type builder to check for JSON properties.</param>
    /// <returns>True if the entity has JSON properties; otherwise, false.</returns>
    public static bool HasJsonProperties(this IMutableEntityType entity) =>
        entity.ClrType.GetProperties()
            .Any(p => Attribute.IsDefined(p, typeof(JsonAttribute), inherit: true));

    /// <summary>
    /// Applies JSON conversion to properties of a specific entity through its builder.
    /// </summary>
    /// <param name="entityTypeBuilder">The entity type builder to apply the JSON conversion to.</param>
    /// <param name="databaseType">The database provider, if specific configuration is needed.</param>
    public static void ApplyJsonConversion(this EntityTypeBuilder entityTypeBuilder, DatabaseType databaseType)
    {
        var properties = entityTypeBuilder.Metadata.ClrType.GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(JsonAttribute), inherit: true))
            .ToList();

        foreach (var property in properties)
        {
            var propertyBuilder = entityTypeBuilder.Property(property.Name);
            var converterType = typeof(JsonValueConverter<>).MakeGenericType(property.PropertyType);
            var comparerType = typeof(JsonValueComparer<>).MakeGenericType(property.PropertyType);
            var converter = Activator.CreateInstance(converterType) as ValueConverter;
            var comparer = Activator.CreateInstance(comparerType) as ValueComparer;

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            switch (databaseType)
            {
                case DatabaseType.Sqlite:
                    propertyBuilder.HasColumnType("TEXT");
                    break;
                case DatabaseType.SqlServer:
                    propertyBuilder.HasColumnType("json");
                    break;
                case DatabaseType.PostgreSql:
                    propertyBuilder.HasColumnType("jsonb");
                    break;
            }
        }
    }

    sealed class JsonValueConverter<T>() : ValueConverter<T?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, _jsonSerializerOptions),
            v => v == null ? default : JsonSerializer.Deserialize<T>(v, _jsonSerializerOptions))
        where T : class;

    sealed class JsonValueComparer<T>() : ValueComparer<T?>(
            (a, b) => JsonEquals(a, b, _jsonSerializerOptions),
            v => v == null ? 0 : JsonSerializer.Serialize(v, _jsonSerializerOptions).GetHashCode(),
            v => v == null ? default : JsonSerializer.Deserialize<T>(
                        JsonSerializer.Serialize(v, _jsonSerializerOptions), _jsonSerializerOptions))
            where T : class
    {
        static bool JsonEquals(T? a, T? b, JsonSerializerOptions opt)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return JsonSerializer.Serialize(a, opt) == JsonSerializer.Serialize(b, opt);
        }
    }
}
