using Cratis.Concepts;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Swagger;

/// <summary>
/// Represents an implementation of <see cref="ISchemaFilter"/> that correctly provides the schema for <see cref="ConceptAs{T}"/>.
/// </summary>
public class ConceptSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;
        if (!type.IsConcept())
        {
            return;
        }

        var valueType = type.GetConceptValueType();
        var newSchema = valueType.MapTypeToOpenApiPrimitiveType();
        schema.Type = newSchema.Type;
        schema.Format = newSchema.Format;
    }
}
