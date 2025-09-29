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
                    context.HandleCorrelationId(correlationIdAccessor, appModelOptions.CorrelationId);

                    var paging = context.GetPagingInfo();
                    var sorting = context.GetSortingInfo();
                    var parameters = context.GetQueryParameters();

                    // Check if we should handle streaming queries for WebSocket
                    var webSocketQueryHandler = context.RequestServices.GetRequiredService<IWebSocketQueryHandler>();
                    if (context.WebSockets.IsWebSocketRequest && await ShouldHandleAsStreamingQuery(performer, context, parameters, paging, sorting))
                    {
                        var streamingResult = await PerformStreamingQuery(performer, parameters, paging, sorting, context);
                        if (streamingResult is not null)
                        {
                            var correlationId = correlationIdAccessor.Current != CorrelationId.NotSet ?
                                correlationIdAccessor.Current : CorrelationId.New();
                            var queryContext = new QueryContext(performer.Name, correlationId, paging, sorting, parameters, []);
                            await webSocketQueryHandler.HandleStreamingResult(context, performer.Name, streamingResult, queryContext);
                            return;
                        }
                    }

                    var queryResult = await queryPipeline.Perform(performer.Name, parameters, paging, sorting);

                    context.Response.SetResponseStatusCode(queryResult);
                    await context.Response.WriteAsJsonAsync(queryResult, jsonSerializerOptions, cancellationToken: context.RequestAborted);
                });
            }
        }

        return app;
    }

    static async Task<bool> ShouldHandleAsStreamingQuery(
        IQueryPerformer performer,
        HttpContext context,
        QueryArguments parameters,
        Paging paging,
        Sorting sorting)
    {
        // For now, we'll perform the query and check if the result is streaming
        // In the future, this could be optimized by checking metadata or other indicators
        try
        {
            var correlationIdAccessor = context.RequestServices.GetRequiredService<ICorrelationIdAccessor>();
            var correlationId = correlationIdAccessor.Current != CorrelationId.NotSet ?
                correlationIdAccessor.Current : CorrelationId.New();
            var dependencies = performer.Dependencies.Select(context.RequestServices.GetRequiredService);
            var queryContext = new QueryContext(performer.Name, correlationId, paging, sorting, parameters, dependencies);
            var result = await performer.Perform(queryContext);
            var webSocketQueryHandler = context.RequestServices.GetRequiredService<IWebSocketQueryHandler>();
            return webSocketQueryHandler.IsStreamingResult(result);
        }
        catch
        {
            return false;
        }
    }

    static async Task<object?> PerformStreamingQuery(
        IQueryPerformer performer,
        QueryArguments parameters,
        Paging paging,
        Sorting sorting,
        HttpContext context)
    {
        var correlationIdAccessor = context.RequestServices.GetRequiredService<ICorrelationIdAccessor>();
        var correlationId = correlationIdAccessor.Current != CorrelationId.NotSet ?
            correlationIdAccessor.Current : CorrelationId.New();
        var dependencies = performer.Dependencies.Select(context.RequestServices.GetRequiredService);
        var queryContext = new QueryContext(performer.Name, correlationId, paging, sorting, parameters, dependencies);
        return await performer.Perform(queryContext);
    }
}