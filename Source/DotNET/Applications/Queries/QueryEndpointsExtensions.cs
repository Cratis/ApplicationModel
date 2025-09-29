// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications;
using Cratis.Applications.Execution;
using Cratis.Applications.Queries;
using Cratis.Execution;
using Cratis.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Provides extension methods for adding query endpoints.
/// </summary>
public static class QueryEndpointsExtensions
{
    /// <summary>
    /// Use Cratis query endpoints.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/> to extend.</param>
    /// <returns><see cref="IApplicationBuilder"/> for continuation.</returns>
    public static IApplicationBuilder UseQueryEndpoints(this IApplicationBuilder app)
    {
        app.UseRouting();

        if (app is IEndpointRouteBuilder endpoints)
        {
            var appModelOptions = app.ApplicationServices.GetRequiredService<IOptions<ApplicationModelOptions>>().Value;
            var options = appModelOptions.Queries;
            var correlationIdAccessor = app.ApplicationServices.GetRequiredService<ICorrelationIdAccessor>();
            var queryPipeline = app.ApplicationServices.GetRequiredService<IQueryPipeline>();
            var queryPerformerProviders = app.ApplicationServices.GetRequiredService<IQueryPerformerProviders>();
            var jsonSerializerOptions = Globals.JsonSerializerOptions;

            var prefix = options.RoutePrefix.Trim('/');
            var group = endpoints.MapGroup($"/{prefix}").WithTags("Queries").WithOpenApi();

            foreach (var performer in queryPerformerProviders.Performers)
            {
                var segments = performer.Location.Skip(options.SegmentsToSkipForRoute);
                var baseUrl = $"/{string.Join('/', segments)}";
                var url = options.IncludeQueryNameInRoute ? $"{baseUrl}/{performer.Name}" : baseUrl;
                url = url.ToLowerInvariant();

                group.MapGet(url, async context =>
                {
                    CorrelationIdHelpers.Handle(correlationIdAccessor, appModelOptions.CorrelationId, context);

                    var paging = context.GetPagingInfo();
                    var sorting = context.GetSortingInfo();
                    var parameters = context.GetQueryParameters();

                    var queryResult = await queryPipeline.Perform(performer.Name, parameters, paging, sorting);

                    context.Response.SetResponseStatusCode(queryResult);
                    await context.Response.WriteAsJsonAsync(queryResult, jsonSerializerOptions, cancellationToken: context.RequestAborted);
                });
            }
        }

        return app;
    }
}