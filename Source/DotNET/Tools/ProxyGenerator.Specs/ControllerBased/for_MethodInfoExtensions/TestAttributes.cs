// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.ControllerBased.for_MethodInfoExtensions;

[AttributeUsage(AttributeTargets.Class)]
public class RouteAttribute(string template) : Attribute
{
    public string Template { get; } = template;
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpGetAttribute : Attribute
{
    public HttpGetAttribute() { }
    public HttpGetAttribute(string template) => Template = template;
    public string? Template { get; }
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpPostAttribute : Attribute
{
    public HttpPostAttribute() { }
    public HttpPostAttribute(string template) => Template = template;
    public string? Template { get; }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class FromRouteAttribute : Attribute;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class FromQueryAttribute : Attribute;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromRequestAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class AspNetResultAttribute : Attribute;
