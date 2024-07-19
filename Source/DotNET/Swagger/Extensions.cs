// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Extension methods for setting up Swagger for a Cratis application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds the <see cref="ConceptSchemaFilter"/> <see cref="ISchemaFilter"/>.
    /// </summary>
    /// <param name="options">The <see cref="SwaggerGenOptions"/>.</param>
    public static void AddConcepts(this SwaggerGenOptions options)
    {
        options.SchemaFilter<ConceptSchemaFilter>();
        options.OperationFilter<CommandResultOperationFilter>();
        options.OperationFilter<QueryResultOperationFilter>();
    }
}
