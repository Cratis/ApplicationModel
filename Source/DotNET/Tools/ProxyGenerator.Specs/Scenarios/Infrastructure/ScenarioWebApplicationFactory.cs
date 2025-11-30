// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// A test web application factory for scenario testing that sets up Arc infrastructure.
/// </summary>
/// <typeparam name="TScenario">The scenario type containing controllers and models.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ScenarioWebApplicationFactory{TScenario}"/> class.
/// </remarks>
/// <param name="configureServices">Optional service configuration callback.</param>
/// <param name="configureApp">Optional application configuration callback.</param>
public class ScenarioWebApplicationFactory<TScenario>(
    Action<IServiceCollection>? configureServices = null,
    Action<WebApplication>? configureApp = null) : WebApplicationFactory<TScenario>
    where TScenario : class
{
    readonly Action<IServiceCollection>? _configureServices = configureServices;
    readonly Action<WebApplication>? _configureApp = configureApp;

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Add controllers from the scenario assembly
            services.AddControllers()
                .AddApplicationPart(typeof(TScenario).Assembly);

            _configureServices?.Invoke(services);
        });
    }

    /// <inheritdoc/>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureWebHost(webBuilder =>
        {
            webBuilder.Configure(app =>
            {
                app.UseRouting();
                app.UseCratisArc();

                if (app is WebApplication webApp)
                {
                    _configureApp?.Invoke(webApp);
                }

                app.UseEndpoints(endpoints => endpoints.MapControllers());
            });
        });

        return base.CreateHost(builder);
    }
}
