// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// Helper class for accessing embedded resources.
/// </summary>
public static class EmbeddedResources
{
    const string TypeScriptCompilerResource = "Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure.typescript.min.js";
    const string ArcRuntimeResource = "Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure.arc-runtime.js";

    /// <summary>
    /// Gets the TypeScript compiler JavaScript code.
    /// </summary>
    /// <returns>The TypeScript compiler code.</returns>
    /// <exception cref="InvalidOperationException">The exception that is thrown when the TypeScript compiler resource is not found.</exception>
    public static string GetTypeScriptCompiler()
    {
        return GetEmbeddedResource(TypeScriptCompilerResource)
            ?? throw new InvalidOperationException($"TypeScript compiler not found. Ensure '{TypeScriptCompilerResource}' is embedded as a resource. " +
                                                   "Download it from https://www.typescriptlang.org/ and place it in the Infrastructure folder.");
    }

    /// <summary>
    /// Gets the Arc runtime shim code.
    /// </summary>
    /// <returns>The Arc runtime code.</returns>
    /// <exception cref="InvalidOperationException">The exception that is thrown when the Arc runtime resource is not found.</exception>
    public static string GetArcRuntime()
    {
        return GetEmbeddedResource(ArcRuntimeResource)
            ?? throw new InvalidOperationException($"Arc runtime not found. Ensure '{ArcRuntimeResource}' is embedded as a resource.");
    }

    static string? GetEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
