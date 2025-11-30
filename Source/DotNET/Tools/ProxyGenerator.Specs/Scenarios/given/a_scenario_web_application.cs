// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cratis.Arc.ProxyGenerator.Scenarios.given;

/// <summary>
/// A reusable context that sets up the scenario web application with Arc infrastructure.
/// </summary>
public class a_scenario_web_application : Specification, IDisposable
{
    /// <summary>
    /// Gets or sets the web application factory.
    /// </summary>
    protected IHost? Host { get; set; }

    /// <summary>
    /// Gets or sets the HTTP client for making requests.
    /// </summary>
    protected HttpClient? HttpClient { get; set; }

    /// <summary>
    /// Gets or sets the JavaScript runtime.
    /// </summary>
    protected JavaScriptRuntime? Runtime { get; set; }

    /// <summary>
    /// Gets or sets the JavaScript-HTTP bridge.
    /// </summary>
    protected JavaScriptHttpBridge? Bridge { get; set; }

    void Establish()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        // Suppress verbose logging during tests
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(a_scenario_web_application).Assembly);

        builder.Services.AddRouting();
        builder.Host.AddCratisArc();

        var app = builder.Build();

        app.UseRouting();
        app.UseCratisArc();
        app.MapControllers();

        // Start the application
        Host = app;
        app.Start();

        HttpClient = app.GetTestClient();
        Runtime = new JavaScriptRuntime();
        Bridge = new JavaScriptHttpBridge(Runtime, HttpClient);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Bridge?.Dispose();
            HttpClient?.Dispose();
            Host?.Dispose();
        }
    }
}
