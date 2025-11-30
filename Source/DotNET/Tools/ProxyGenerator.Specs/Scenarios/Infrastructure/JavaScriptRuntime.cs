// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.ClearScript.V8;

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// Represents a JavaScript runtime environment using V8 engine with TypeScript transpilation support.
/// </summary>
public sealed class JavaScriptRuntime : IDisposable
{
    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="JavaScriptRuntime"/> class.
    /// </summary>
    public JavaScriptRuntime()
    {
        Engine = new V8ScriptEngine();
        InitializeRuntime();
    }

    /// <summary>
    /// Gets the underlying V8 script engine.
    /// </summary>
    public V8ScriptEngine Engine { get; }

    /// <summary>
    /// Transpiles TypeScript code to JavaScript.
    /// </summary>
    /// <param name="typeScriptCode">The TypeScript code to transpile.</param>
    /// <returns>The transpiled JavaScript code.</returns>
    public string TranspileTypeScript(string typeScriptCode)
    {
        var escapedCode = typeScriptCode.Replace("\\", "\\\\").Replace("`", "\\`").Replace("$", "\\$");
        var result = Engine.Evaluate($"ts.transpile(`{escapedCode}`, {{ target: ts.ScriptTarget.ES2020, module: ts.ModuleKind.CommonJS }})");
        return result?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Executes JavaScript code in the runtime.
    /// </summary>
    /// <param name="javaScriptCode">The JavaScript code to execute.</param>
    public void Execute(string javaScriptCode)
    {
        Engine.Execute(javaScriptCode);
    }

    /// <summary>
    /// Executes JavaScript code and returns the result.
    /// </summary>
    /// <typeparam name="T">The expected return type.</typeparam>
    /// <param name="javaScriptCode">The JavaScript code to execute.</param>
    /// <returns>The result of the execution.</returns>
    public T? Evaluate<T>(string javaScriptCode)
    {
        var result = Engine.Evaluate(javaScriptCode);
        if (result is T typedResult)
        {
            return typedResult;
        }

        return default;
    }

    /// <summary>
    /// Executes JavaScript code and returns the raw result.
    /// </summary>
    /// <param name="javaScriptCode">The JavaScript code to execute.</param>
    /// <returns>The result of the execution.</returns>
    public object? Evaluate(string javaScriptCode)
    {
        return Engine.Evaluate(javaScriptCode);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            Engine.Dispose();
            _disposed = true;
        }
    }

    void InitializeRuntime()
    {
        // Load TypeScript compiler
        var typeScriptCompiler = EmbeddedResources.GetTypeScriptCompiler();
        Engine.Execute(typeScriptCompiler);

        // Load Arc runtime shims
        var arcRuntime = EmbeddedResources.GetArcRuntime();
        Engine.Execute(arcRuntime);
    }
}
