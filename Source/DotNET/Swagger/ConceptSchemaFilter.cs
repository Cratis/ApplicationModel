// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Represents an implementation of <see cref="ISchemaFilter"/> that correctly provides the schema for <see cref="ConceptAs{T}"/>.
/// </summary>
public class ConceptSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;
        if (!type.IsConcept())
        {
            return;
        }

        if (schema is not OpenApiSchema concreteSchema)
        {
            return;
        }

        var valueType = type.GetConceptValueType();
        var (jsonType, format) = GetJsonSchemaTypeForType(valueType);
        concreteSchema.Type = jsonType;
        concreteSchema.Format = format;
    }

    static (JsonSchemaType, string?) GetJsonSchemaTypeForType(Type type)
    {
        if (type == typeof(string))
            return (JsonSchemaType.String, null);
        if (type == typeof(int) || type == typeof(long))
            return (JsonSchemaType.Integer, type == typeof(long) ? "int64" : "int32");
        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            return (JsonSchemaType.Number, type == typeof(float) ? "float" : type == typeof(double) ? "double" : null);
        if (type == typeof(bool))
            return (JsonSchemaType.Boolean, null);
        if (type == typeof(Guid))
            return (JsonSchemaType.String, "uuid");
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            return (JsonSchemaType.String, "date-time");

        return (JsonSchemaType.Object, null);
    }
}
