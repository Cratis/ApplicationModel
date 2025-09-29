// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using Cratis.Applications.Queries;
using Cratis.Concepts;
using Cratis.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Represents an implementation of <see cref="IOperationFilter"/> that adds the command result to the operation for command methods.
/// </summary>
public class QueryResultOperationFilter : IOperationFilter
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

        var queryResultType = typeof(QueryResult);

        if (context.MethodInfo.ReturnType.IsEnumerable())
        {
            QueryParameterUtilities.AddPagingAndSortingParameters(operation);
        }

        var schema = context.SchemaGenerator.GenerateSchema(queryResultType, context.SchemaRepository);
        var response = operation.Responses.First((kvp) => kvp.Key == ((int)HttpStatusCode.OK).ToString()).Value;
        if (response.Content.TryGetValue("application/json", out var value))
        {
            value.Schema = schema;
        }
        else
        {
            response.Content.Add(new("application/json", new() { Schema = schema }));
        }

        operation.Responses.Add(((int)HttpStatusCode.Forbidden).ToString(), new OpenApiResponse()
        {
            Description = "Forbidden",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "application/json", new OpenApiMediaType() { Schema = schema } }
            }
        });

        operation.Responses.Add(((int)HttpStatusCode.BadRequest).ToString(), new OpenApiResponse()
        {
            Description = "Bad Request - typically a validation error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "application/json", new OpenApiMediaType() { Schema = schema } }
            }
        });

        operation.Responses.Add(((int)HttpStatusCode.InternalServerError).ToString(), new OpenApiResponse()
        {
            Description = "Internal server error - something went wrong. See the exception details.",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                { "application/json", new OpenApiMediaType() { Schema = schema } }
            }
        });
    }
}
