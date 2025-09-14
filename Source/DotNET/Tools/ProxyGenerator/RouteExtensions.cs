// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Extension methods for working with routes.
/// </summary>
public static class RouteExtensions
{
    /// <summary>
    /// Make a route template from a route.
    /// </summary>
    /// <param name="route">String representing the route.</param>
    /// <returns>A template version of the route.</returns>
    public static string MakeRouteTemplate(this string route) =>
        route.Replace("{", "{{").Replace("}", "}}");
}
