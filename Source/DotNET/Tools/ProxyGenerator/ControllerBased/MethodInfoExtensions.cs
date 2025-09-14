// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator.ControllerBased;

/// <summary>
/// Extension methods for <see cref="MethodInfo"/>.
/// </summary>
public static class MethodInfoExtensions
{
    /// <summary>
    /// Get the route for a method.
    /// </summary>
    /// <param name="method">Method to get for.</param>
    /// <param name="arguments">Arguments for the method.</param>
    /// <returns>The full route.</returns>
    public static string GetRoute(this MethodInfo method, IEnumerable<RequestArgumentDescriptor> arguments)
    {
        var routeTemplates = new string[]
        {
            method.DeclaringType?.GetAttributeConstructorArgument(WellKnownTypes.RouteAttribute, 0)?.ToString() ?? string.Empty,
            method.GetAttributeConstructorArgument(WellKnownTypes.HttpGetAttribute, 0)?.ToString() ?? string.Empty,
            method.GetAttributeConstructorArgument(WellKnownTypes.HttpPostAttribute, 0)?.ToString() ?? string.Empty
        };

        var route = string.Empty;

        foreach (var template in routeTemplates)
        {
            route = $"{route}/{template}".Trim('/');
        }

        if (!route.StartsWith('/')) route = $"/{route}";

        var queryStringParameters = arguments.Where(argument => argument.IsQueryStringParameter).ToArray();
        if (queryStringParameters.Length > 0)
        {
            var queryString = string.Join('&', queryStringParameters.Select(_ => $"{_.Name}={{{_.Name}}}"));
            route = $"{route}?{queryString}";
        }
        return route;
    }

    /// <summary>
    /// Get argument descriptors for a method.
    /// </summary>
    /// <param name="methodInfo">Method to get for.</param>
    /// <returns>Collection of <see cref="RequestArgumentDescriptor"/>. </returns>
    public static IEnumerable<RequestArgumentDescriptor> GetArgumentDescriptors(this MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();

        return
        [
            .. parameters.Where(_ => _.IsRequestArgument()).Select(_ => _.ToRequestArgumentDescriptor()),
            .. parameters.Where(_ => _.IsFromRequestArgument()).SelectMany(_ => _.GetRequestArgumentDescriptors())
        ];
    }

    /// <summary>
    /// Check if a method is a query method.
    /// </summary>
    /// <param name="method">Method to check.</param>
    /// <returns>True if it is a query method, false otherwise.</returns>
    public static bool IsQueryMethod(this MethodInfo method)
    {
        var attributes = method.GetCustomAttributesData().Select(_ => _.AttributeType.Name);
        return attributes.Any(_ => _ == WellKnownTypes.HttpGetAttribute) &&
            !attributes.Any(_ => _ == WellKnownTypes.AspNetResultAttribute);
    }

    /// <summary>
    /// Check if a method is a query method.
    /// </summary>
    /// <param name="method">Method to check.</param>
    /// <returns>True if it is a query method, false otherwise.</returns>
    public static bool IsCommandMethod(this MethodInfo method)
    {
        var attributes = method.GetCustomAttributesData().Select(_ => _.AttributeType.Name);
        return attributes.Any(_ => _ == WellKnownTypes.HttpPostAttribute) &&
            !attributes.Any(_ => _ == WellKnownTypes.AspNetResultAttribute);
    }

    /// <summary>
    /// Get properties from a <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">Method to get for.</param>
    /// <returns>Collection of <see cref="PropertyDescriptor"/>.</returns>
    public static IEnumerable<PropertyDescriptor> GetPropertyDescriptors(this MethodInfo method)
    {
        List<PropertyDescriptor> properties = [];
        var parameters = method.GetParameters();
        var primitives = parameters.Where(_ => _.ParameterType.IsAPrimitiveType() || _.ParameterType.IsConcept());
        var complex = parameters.Where(_ => !_.ParameterType.IsAPrimitiveType() && !_.ParameterType.IsConcept());

        properties.AddRange(primitives.ToList().ConvertAll(_ => _.ToPropertyDescriptor()));

        foreach (var complexType in complex)
        {
            properties.AddRange(complexType.ParameterType.GetProperties().Select(_ => _.ToPropertyDescriptor()));
        }

        return properties;
    }

    /// <summary>
    /// Get the constructor argument for an attribute.
    /// </summary>
    /// <param name="member">Member to get from.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="index">Optional argument index.</param>
    /// <returns>Argument if it was found, false if not.</returns>
    public static object? GetAttributeConstructorArgument(this MemberInfo member, string attributeName, int index = 0)
    {
        var attribute = member.GetCustomAttributesData().FirstOrDefault(_ => _.AttributeType.Name == attributeName);
        if (attribute is null) return null;
        if (attribute.ConstructorArguments.Count <= index) return null;
        return attribute.ConstructorArguments[index].Value;
    }
}
