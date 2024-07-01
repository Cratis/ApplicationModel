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
