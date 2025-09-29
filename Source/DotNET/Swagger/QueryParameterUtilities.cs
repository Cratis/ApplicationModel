// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Queries;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Provides utility methods for adding standard query parameters to Swagger operations.
/// </summary>
public static class QueryParameterUtilities
{
    /// <summary>
    /// Adds standard paging and sorting parameters to the operation for enumerable query results.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to add parameters to.</param>
    public static void AddPagingAndSortingParameters(OpenApiOperation operation)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = QueryHttpExtensions.SortByQueryStringKey,
            In = ParameterLocation.Query,
            Description = "Sort by field name",
            Required = false,
            Schema = new OpenApiSchema { Type = "string" }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = QueryHttpExtensions.SortDirectionQueryStringKey,
            In = ParameterLocation.Query,
            Required = false,
            Description = "Sort direction",
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = [new OpenApiString("asc"), new OpenApiString("desc")]
            }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = QueryHttpExtensions.PageSizeQueryStringKey,
            In = ParameterLocation.Query,
            Description = "Number of items to limit a page to",
            Required = false,
            Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = QueryHttpExtensions.PageQueryStringKey,
            In = ParameterLocation.Query,
            Description = "Page number to show",
            Required = false,
            Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
        });
    }
}