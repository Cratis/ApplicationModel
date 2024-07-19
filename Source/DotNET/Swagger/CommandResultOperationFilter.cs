// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Commands;
using Cratis.Concepts;
using Cratis.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Represents an implementation of <see cref="IOperationFilter"/> that adds the command result to the operation for command methods.
/// </summary>
/// <param name="schemaGenerator">The <see cref="ISchemaGenerator"/> to use.</param>
public class CommandResultOperationFilter(ISchemaGenerator schemaGenerator) : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.MethodInfo.IsCommand()) return;

        var returnType = context.MethodInfo.GetActualReturnType();

        Type? commandResultType = null;

        if (returnType == typeof(Task) || returnType == typeof(ValueTask) || returnType == typeof(void))
        {
            returnType = typeof(object);
            commandResultType = typeof(CommandResult);
        }
        else if (returnType.IsConcept())
        {
            returnType = returnType.GetConceptValueType();
        }

        commandResultType ??= typeof(CommandResult<>).MakeGenericType(returnType);

        var schema = schemaGenerator.GenerateSchema(commandResultType, context.SchemaRepository);
        var response = operation.Responses.First().Value;
        if (response.Content.ContainsKey("application/json"))
        {
            operation.Responses.First().Value.Content["application/json"].Schema = schema;
        }
        else
        {
            response.Content.Add(new("application/json", new() { Schema = schema }));
        }
    }
}
