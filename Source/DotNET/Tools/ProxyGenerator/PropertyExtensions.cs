// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Extension methods for working with properties.
/// </summary>
public static class PropertyExtensions
{
    /// <summary>
    /// Convert a <see cref="PropertyInfo"/> to a <see cref="PropertyDescriptor"/>.
    /// </summary>
    /// <param name="property">Property to convert.</param>
    /// <returns><see cref="PropertyDescriptor"/>.</returns>
    public static PropertyDescriptor ToPropertyDescriptor(this PropertyInfo property) =>
        ToPropertyDescriptor(property.PropertyType, property.Name);

    /// <summary>
    /// Check if a property is a request argument which will make it part of the query string either as route variable or a query string parameter.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <returns>True if it is a request argument, false otherwise.</returns>
    public static bool IsRequestArgument(this PropertyInfo property)
    {
        var attributes = property.GetCustomAttributesData().Select(_ => _.AttributeType.Name);
        return attributes.Any(_ => _ == WellKnownTypes.FromRouteAttribute) ||
               attributes.Any(_ => _ == WellKnownTypes.FromQueryAttribute);
    }

    /// <summary>
    /// Check if a property is adorned with the FromQueryAttribute, which means we need to treat it as a query string parameter and include it in the route template.
    /// </summary>
    /// <param name="property">Method to check.</param>
    /// <returns>True if it is a from request argument, false otherwise.</returns>
    public static bool IsFromQueryArgument(this PropertyInfo property)
    {
        var attributes = property.GetCustomAttributesData().Select(_ => _.AttributeType.Name);
        return attributes.Any(_ => _ == WellKnownTypes.FromQueryAttribute);
    }

    /// <summary>
    /// Convert a <see cref="PropertyInfo"/> to a <see cref="RequestArgumentDescriptor"/>.
    /// </summary>
    /// <param name="propertyInfo">Parameter to convert.</param>
    /// <returns>Converted <see cref="RequestArgumentDescriptor"/>.</returns>
    public static RequestArgumentDescriptor ToRequestArgumentDescriptor(this PropertyInfo propertyInfo)
    {
        var type = propertyInfo.PropertyType.GetTargetType();
        var optional = propertyInfo.IsOptional();
        return new RequestArgumentDescriptor(
            propertyInfo.PropertyType,
            propertyInfo.Name!,
            type.Type,
            optional,
            propertyInfo.IsFromQueryArgument());
    }

    /// <summary>
    /// Check if a property is optional - typically for arguments or properties.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <returns>True if it is, false if not.</returns>
    public static bool IsOptional(this PropertyInfo property)
    {
        return property.CustomAttributes.Any(_ =>
            _.AttributeType.FullName?.StartsWith("System.Runtime.CompilerServices.NullableAttribute") ?? false);
    }

    /// <summary>
    /// Convert a <see cref="ParameterInfo"/> to a <see cref="PropertyDescriptor"/>.
    /// </summary>
    /// <param name="parameterInfo">Parameter to convert.</param>
    /// <returns><see cref="PropertyDescriptor"/>.</returns>
    public static PropertyDescriptor ToPropertyDescriptor(this ParameterInfo parameterInfo) =>
        ToPropertyDescriptor(parameterInfo.ParameterType, parameterInfo.Name!);

    static PropertyDescriptor ToPropertyDescriptor(Type propertyType, string name)
    {
        var isEnumerable = false;
        var isNullable = false;
        if (!propertyType.IsKnownType())
        {
            isEnumerable = propertyType.IsEnumerable();
            if (isEnumerable)
            {
                propertyType = propertyType.GetEnumerableElementType()!;
            }
            isNullable = propertyType.IsNullable();
            if (isNullable)
            {
                propertyType = propertyType.GetGenericArguments()[0];
            }
        }

        var targetType = propertyType.GetTargetType();

        return new(
            propertyType,
            name,
            targetType.Type,
            targetType.Constructor,
            targetType.Module,
            isEnumerable,
            isNullable,
            propertyType.IsAPrimitiveType());
    }
}
