// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications;
using Cratis.Applications.Commands;
using Cratis.Applications.Execution;
using Cratis.Execution;
using Cratis.Json;
using Microsoft.AspNetCore.Http;
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
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            var appModelOptions = app.ApplicationServices.GetRequiredService<IOptions<ApplicationModelOptions>>().Value;
            var options = appModelOptions.Commands;
            var correlationIdAccessor = app.ApplicationServices.GetRequiredService<ICorrelationIdAccessor>();
            var commandPipeline = app.ApplicationServices.GetRequiredService<ICommandPipeline>();
            var commandHandlerProviders = app.ApplicationServices.GetRequiredService<ICommandHandlerProviders>();
            var jsonSerializerOptions = Globals.JsonSerializerOptions;

            var prefix = options.RoutePrefix.Trim('/');
            var group = endpoints.MapGroup($"/{prefix}").WithTags("Commands").WithOpenApi();

            foreach (var handler in commandHandlerProviders.Handlers)
            {
                var segments = handler.Location.Skip(options.SegmentsToSkipForRoute);
                var baseUrl = $"/{string.Join('/', segments)}";
                var url = options.IncludeCommandNameInRoute ? $"{baseUrl}/{handler.CommandType.Name}" : baseUrl;
                url = url.ToLowerInvariant();
                group.MapPost(url, async context =>
                {
                    CorrelationIdHelpers.Handle(correlationIdAccessor, appModelOptions.CorrelationId, context);
                    var command = await context.Request.ReadFromJsonAsync(handler.CommandType, jsonSerializerOptions, cancellationToken: context.RequestAborted);
                    CommandResult commandResult;
                    if (command is null)
                    {
                        commandResult = CommandResult.Error($"Could not deserialize command of type '{handler.CommandType}' from request body.");
                    }
                    else
                    {
                        commandResult = await commandPipeline.Execute(command);
                    }
                    context.Response.SetResponseStatusCode(commandResult);
                    await context.Response.WriteAsJsonAsync(commandResult, jsonSerializerOptions, cancellationToken: context.RequestAborted);
                });
            }
        });

        return app;
    }
}
