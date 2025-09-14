// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator.ModelBound;

/// <summary>
/// Extensions for command types.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Convert a command <see cref="TypeInfo"/> to a <see cref="CommandDescriptor"/>.
    /// </summary>
    /// <param name="commandType">Command type to convert.</param>
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <param name="skipCommandNameInRoute">True if the command name should be skipped in the route, false if not.</param>
    /// <param name="apiPrefix">The API prefix to use in the route.</param>
    /// <returns>Converted <see cref="CommandDescriptor"/>.</returns>
    public static CommandDescriptor ToCommandDescriptor(this TypeInfo commandType, string targetPath, int segmentsToSkip, bool skipCommandNameInRoute, string apiPrefix)
    {
        var properties = commandType.GetPropertyDescriptors();
        var location = commandType.Namespace?.Split('.') ?? [];
        var segments = location.Skip(segmentsToSkip);
        var baseUrl = $"/{apiPrefix}/{string.Join('/', segments)}";
        var route = skipCommandNameInRoute ? baseUrl : $"{baseUrl}/{commandType.Name}";
        route = route.ToLowerInvariant();
        var handleMethod = commandType.GetHandleMethod();

        return handleMethod.ToCommandDescriptor(commandType.Name, properties, [], route, targetPath, segmentsToSkip);
    }
}