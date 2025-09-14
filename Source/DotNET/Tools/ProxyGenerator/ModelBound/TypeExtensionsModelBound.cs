// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Applications.ProxyGenerator.ModelBound;

/// <summary>
/// Extension methods for working with model bound types.
/// </summary>
public static class TypeExtensionsModelBound
{
    /// <summary>
    /// Determine if a type is a model bound command.
    /// </summary>
    /// <param name="type">Type to inspect.</param>
    /// <returns>True if the type is a model bound command, false otherwise.</returns>
    public static bool IsCommand(this Type type) =>
        type.IsClass &&
        !type.IsAbstract &&
        type.GetCustomAttributesData()
            .Select(_ => _.AttributeType.Name)
            .Any(_ => _ == WellKnownTypes.CommandAttribute) &&
        type.HasHandleMethod();

    /// <summary>
    /// Determine if a type has a Handle method.
    /// </summary>
    /// <param name="type">Type to inspect.</param>
    /// <returns>True if the type has a Handle method, false otherwise.</returns>
    public static bool HasHandleMethod(this Type type) =>
        type.GetMethods().SingleOrDefault(_ => _.Name == "Handle") != null;

    /// <summary>
    /// Get the Handle method from a type.
    /// </summary>
    /// <param name="type">Type to inspect.</param>
    /// <returns>The Handle method.</returns>
    public static MethodInfo GetHandleMethod(this Type type) =>
        type.GetMethods().Single(_ => _.Name == "Handle");
}
