// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reflection;

namespace Cratis.Applications.Queries.ModelBound;

/// <summary>
/// Represents a model bound query performer.
/// </summary>
/// <param name="readModelType">The type of the read model.</param>
/// <param name="performMethod">The method info of the perform method.</param>
public class ModelBoundQueryPerformer(Type readModelType, MethodInfo performMethod) : IQueryPerformer
{
    /// <inheritdoc/>
    public QueryName Name { get; } = $"{readModelType.FullName}.{performMethod.Name}";

    /// <inheritdoc/>
    public IEnumerable<string> Location { get; } = readModelType.Namespace?.Split('.') ?? [];

    /// <inheritdoc/>
    public IEnumerable<Type> Dependencies { get; } = performMethod.GetParameters().Select(p => p.ParameterType);

    /// <inheritdoc/>
    public async Task<object?> Perform(QueryContext context)
    {
        var parameters = performMethod.GetParameters();
        var args = new object?[parameters.Length];

        var dependencies = context.Dependencies?.ToArray() ?? [];
        var dependencyIndex = 0;

        var queryStringParameters = context.Parameters ?? QueryArguments.Empty;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            var matchingQueryParam = queryStringParameters.FirstOrDefault(kvp =>
                string.Equals(kvp.Key, parameter.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(matchingQueryParam.Key))
            {
                args[i] = ConvertParameterValue(matchingQueryParam.Value, parameter.ParameterType);
            }
            else if (dependencyIndex < dependencies.Length)
            {
                args[i] = dependencies[dependencyIndex];
                dependencyIndex++;
            }
        }

        var result = performMethod.Invoke(null, args);

        if (result is null)
        {
            return null;
        }

        if (result is Task task)
        {
            if (performMethod.ReturnType != typeof(Task))
            {
                var type = task.GetType();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var property = type.GetProperty(nameof(Task<object>.Result));
                    await task;
                    return property?.GetValue(task);
                }
            }
            await task;
            return null;
        }

        return result;
    }

    static object? ConvertParameterValue(object value, Type targetType)
    {
        if (value is null)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        // If the value is already the target type, return it directly
        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }

        // Handle concepts first
        if (targetType.IsConcept())
        {
            var underlyingType = targetType.GetConceptValueType();
            var convertedValue = ConvertToUnderlyingType(value, underlyingType);
            if (convertedValue is not null)
            {
                return ConceptFactory.CreateConceptInstance(targetType, convertedValue);
            }
            return null;
        }

        return ConvertToUnderlyingType(value, targetType);
    }

    static object? ConvertToUnderlyingType(object value, Type targetType)
    {
        if (value is null)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        // If the value is already the target type, return it directly
        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (underlyingType == typeof(string))
        {
            return stringValue;
        }

        try
        {
            if (underlyingType == typeof(int))
                return int.Parse(stringValue);
            if (underlyingType == typeof(long))
                return long.Parse(stringValue);
            if (underlyingType == typeof(short))
                return short.Parse(stringValue);
            if (underlyingType == typeof(byte))
                return byte.Parse(stringValue);
            if (underlyingType == typeof(bool))
                return bool.Parse(stringValue);
            if (underlyingType == typeof(float))
                return float.Parse(stringValue, System.Globalization.CultureInfo.InvariantCulture);
            if (underlyingType == typeof(double))
                return double.Parse(stringValue, System.Globalization.CultureInfo.InvariantCulture);
            if (underlyingType == typeof(decimal))
                return decimal.Parse(stringValue, System.Globalization.CultureInfo.InvariantCulture);
            if (underlyingType == typeof(DateTime))
                return DateTime.Parse(stringValue);
            if (underlyingType == typeof(DateTimeOffset))
                return DateTimeOffset.Parse(stringValue);
            if (underlyingType == typeof(Guid))
                return Guid.Parse(stringValue);
            if (underlyingType.IsEnum)
                return Enum.Parse(underlyingType, stringValue, true);

            var converter = TypeDescriptor.GetConverter(underlyingType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromString(stringValue);
            }
        }
        catch (Exception)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
    }
}