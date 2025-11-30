// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// Base context for proxy generation scenarios.
/// </summary>
public abstract class ScenarioContext : IDisposable
{
    bool _disposed;

    /// <summary>
    /// Gets or sets the JavaScript runtime.
    /// </summary>
    protected JavaScriptRuntime? Runtime { get; set; }

    /// <summary>
    /// Gets or sets the HTTP client for making requests to the test server.
    /// </summary>
    protected HttpClient? HttpClient { get; set; }

    /// <summary>
    /// Gets or sets the JavaScript-HTTP bridge for end-to-end testing.
    /// </summary>
    protected JavaScriptHttpBridge? Bridge { get; set; }

    /// <summary>
    /// Gets the list of generated TypeScript code.
    /// </summary>
    protected List<string> GeneratedTypeScript { get; } = [];

    /// <summary>
    /// Loads generated TypeScript code into the runtime.
    /// </summary>
    /// <param name="typeScriptCode">The TypeScript code to load.</param>
    protected void LoadTypeScript(string typeScriptCode)
    {
        GeneratedTypeScript.Add(typeScriptCode);
        Bridge?.LoadTypeScript(typeScriptCode);
    }

    /// <summary>
    /// Generates and loads a command proxy.
    /// </summary>
    /// <param name="descriptor">The command descriptor.</param>
    protected void LoadCommand(CommandDescriptor descriptor)
    {
        var code = InMemoryProxyGenerator.GenerateCommand(descriptor);
        LoadTypeScript(code);
    }

    /// <summary>
    /// Generates and loads a query proxy.
    /// </summary>
    /// <param name="descriptor">The query descriptor.</param>
    protected void LoadQuery(QueryDescriptor descriptor)
    {
        var code = InMemoryProxyGenerator.GenerateQuery(descriptor);
        LoadTypeScript(code);
    }

    /// <summary>
    /// Generates and loads a type proxy.
    /// </summary>
    /// <param name="descriptor">The type descriptor.</param>
    protected void LoadType(TypeDescriptor descriptor)
    {
        var code = InMemoryProxyGenerator.GenerateType(descriptor);
        LoadTypeScript(code);
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
        if (!_disposed && disposing)
        {
            Bridge?.Dispose();
            HttpClient?.Dispose();
            _disposed = true;
        }
    }
}
