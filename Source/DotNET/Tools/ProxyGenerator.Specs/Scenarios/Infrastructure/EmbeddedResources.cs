// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// Helper class for accessing embedded resources.
/// </summary>
public static class EmbeddedResources
{
    const string ArcBootstrapResource = "Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure.arc-bootstrap.js";

    /// <summary>
    /// Gets the Arc bootstrap code that sets up the module environment.
    /// </summary>
    /// <returns>The Arc bootstrap code.</returns>
    /// <exception cref="InvalidOperationException">The exception that is thrown when the Arc bootstrap resource is not found.</exception>
    public static string GetArcBootstrap()
    {
        return GetEmbeddedResource(ArcBootstrapResource)
            ?? throw new InvalidOperationException($"Arc bootstrap not found. Ensure '{ArcBootstrapResource}' is embedded as a resource.");
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
