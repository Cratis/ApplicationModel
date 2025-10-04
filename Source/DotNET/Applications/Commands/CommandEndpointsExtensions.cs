// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications;
using Cratis.Applications.Commands;
using Cratis.Applications.Execution;
using Cratis.Execution;
using Cratis.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Provides extension methods for adding command endpoints.
/// </summary>
public static class CommandEndpointsExtensions
{
    /// <summary>
    /// Use Cratis default setup.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/> to extend.</param>
    /// <returns><see cref="IApplicationBuilder"/> for continuation.</returns>
    public static IApplicationBuilder UseCommandEndpoints(this IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder endpoints)
        {
            var appModelOptions = app.ApplicationServices.GetRequiredService<IOptions<ApplicationModelOptions>>().Value;
            var options = appModelOptions.GeneratedApis;
            var correlationIdAccessor = app.ApplicationServices.GetRequiredService<ICorrelationIdAccessor>();
            var commandPipeline = app.ApplicationServices.GetRequiredService<ICommandPipeline>();
            var commandHandlerProviders = app.ApplicationServices.GetRequiredService<ICommandHandlerProviders>();
            var jsonSerializerOptions = Globals.JsonSerializerOptions;

            var prefix = options.RoutePrefix.Trim('/');
            var group = endpoints.MapGroup($"/{prefix}");

            foreach (var handler in commandHandlerProviders.Handlers)
            {
                var location = handler.Location.Skip(options.SegmentsToSkipForRoute);
                var segments = location.Select(segment => segment.ToKebabCase());
                var baseUrl = $"/{string.Join('/', segments)}";
                var typeName = options.IncludeCommandNameInRoute ? handler.CommandType.Name : string.Empty;

                var url = options.IncludeCommandNameInRoute ? $"{baseUrl}/{typeName.ToKebabCase()}" : baseUrl;
                url = url.ToLowerInvariant();

                // Note: If we use the minimal API "MapPost" with HttpContext parameter, it does not show up in Swagger
                //       So we use HttpRequest and HttpResponse instead
                group.MapPost(url, async (HttpRequest request, HttpResponse response) =>
                {
                    var context = request.HttpContext;
                    context.HandleCorrelationId(correlationIdAccessor, appModelOptions.CorrelationId);
                    var command = await request.ReadFromJsonAsync(handler.CommandType, jsonSerializerOptions, cancellationToken: context.RequestAborted);
                    CommandResult commandResult;
                    if (command is null)
                    {
                        commandResult = CommandResult.Error(correlationIdAccessor.Current, $"Could not deserialize command of type '{handler.CommandType}' from request body.");
                    }
                    else
                    {
                        commandResult = await commandPipeline.Execute(command);
                    }
                    response.SetResponseStatusCode(commandResult);
                    await response.WriteAsJsonAsync(commandResult, commandResult.GetType(), jsonSerializerOptions, cancellationToken: context.RequestAborted);
                })
                .WithTags(string.Join('.', location))
                .WithName($"Execute{handler.CommandType.Name}")
                .WithSummary($"Execute {handler.CommandType.Name} command")
                .WithOpenApi();
            }
        }

        return app;
    }
}
