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

        if (schema is not OpenApiSchema)
        {
            return;
        }

        var valueType = type.GetConceptValueType();
        var newSchema = valueType.MapTypeToOpenApiPrimitiveType();

        if (schema is OpenApiSchema openApiSchema)
        {
            openApiSchema.Type = newSchema.Type;
            openApiSchema.Format = newSchema.Format;
        }
    }
}
