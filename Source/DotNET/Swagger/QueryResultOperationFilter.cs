// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Queries;
using Cratis.Concepts;
using Cratis.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Represents an implementation of <see cref="IOperationFilter"/> that adds the command result to the operation for command methods.
/// </summary>
/// <param name="schemaGenerator">The <see cref="ISchemaGenerator"/> to use.</param>
public class QueryResultOperationFilter(ISchemaGenerator schemaGenerator) : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.MethodInfo.IsQuery()) return;

        var returnType = context.MethodInfo.GetActualReturnType();

        if (returnType.IsConcept())
        {
            returnType = returnType.GetConceptValueType();
        }

        var queryResultType = typeof(QueryResult<>).MakeGenericType(returnType);

        var schema = schemaGenerator.GenerateSchema(queryResultType, context.SchemaRepository);
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
