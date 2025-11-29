// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Swagger.ModelBound;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up model-bound Swagger filters.
/// </summary>
public static class ModelBoundSwaggerExtensions
{
    /// <summary>
    /// Adds model-bound Swagger operation filters for Commands and Queries minimal APIs.
    /// </summary>
    /// <param name="options">The <see cref="SwaggerGenOptions"/>.</param>
    public static void AddModelBoundOperationFilters(this SwaggerGenOptions options)
    {
        options.OperationFilter<CommandOperationFilter>();
        options.OperationFilter<QueryOperationFilter>();
    }
}
