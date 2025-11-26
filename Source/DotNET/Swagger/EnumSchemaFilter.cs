// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Arc.Swagger;

/// <summary>
/// Represents an implementation of <see cref="ISchemaFilter"/> that correctly provides the schema for enums as names instead of integers.
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum?.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name => schema.Enum?.Add(JsonValue.Create(name)));
        }
    }
}
