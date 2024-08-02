// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.ModelBinding;
using Cratis.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Represents an implementation of <see cref="IOperationFilter"/> that adds support for <see cref="FromRequestAttribute"/> attribute.
/// </summary>
public class FromRequestOperationFilter : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameterInfos = context.MethodInfo.GetParameters();

        var parameters = context.ApiDescription.ParameterDescriptions.Where(_ => parameterInfos.Any(p =>
            p.Name == _.Name && p.HasAttribute<FromRequestAttribute>())).ToArray();
        if (parameters.Length == 0)
        {
            return;
        }

        foreach (var parameter in parameters)
        {
            var openApiParameter = operation.Parameters.FirstOrDefault(_ => _.Name == parameter.Name);
            operation.Parameters.Remove(openApiParameter);

            var schema = context.SchemaGenerator.GenerateSchema(parameter.Type, context.SchemaRepository);
            operation.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = new()
                    {
                        Schema = schema
                    }
                }
            };
        }
    }
}
