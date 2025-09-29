// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Queries.ModelBound;

/// <summary>
/// Represents a model bound query performer.
/// </summary>
public class ModelBoundQueryPerformer : IQueryPerformer
{
    readonly IEnumerable<ParameterInfo> _dependencies;
    readonly IEnumerable<ParameterInfo> _queryParameters;
    readonly MethodInfo _performMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelBoundQueryPerformer"/> class.
    /// </summary>
    /// <param name="readModelType">The type of the read model.</param>
    /// <param name="performMethod">The method info of the perform method.</param>
    /// <param name="serviceProviderIsService">Service to determine if a type is registered as a service.</param>
    public ModelBoundQueryPerformer(Type readModelType, MethodInfo performMethod, IServiceProviderIsService serviceProviderIsService)
    {
        Name = $"{readModelType.FullName}.{performMethod.Name}";
        Location = readModelType.Namespace?.Split('.') ?? [];

        _dependencies = performMethod.GetParameters().Where(p => serviceProviderIsService.IsService(p.ParameterType));
        _queryParameters = performMethod.GetParameters().Where(p => !serviceProviderIsService.IsService(p.ParameterType));
        Dependencies = _dependencies.Select(p => p.ParameterType);
        Parameters = new(_queryParameters.Select(p => new QueryParameter(p.Name ?? string.Empty, p.ParameterType)));
        _performMethod = performMethod;
    }

    /// <inheritdoc/>
    public QueryName Name { get; }

    /// <inheritdoc/>
    public IEnumerable<string> Location { get; }

    /// <inheritdoc/>
    public IEnumerable<Type> Dependencies { get; }

    /// <inheritdoc/>
    public QueryParameters Parameters { get; }

    /// <inheritdoc/>
    public async Task<object?> Perform(QueryContext context)
    {
        var parameters = _performMethod.GetParameters();
        var args = new object?[parameters.Length];

        var dependencies = context.Dependencies?.ToArray() ?? [];
        var dependencyIndex = 0;

        var queryStringParameters = context.Parameters ?? QueryArguments.Empty;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            // Check if this parameter is a dependency (service)
            if (_dependencies.Contains(parameter))
            {
                if (dependencyIndex < dependencies.Length)
                {
                    args[i] = dependencies[dependencyIndex];
                    dependencyIndex++;
                }
            }
            else
            {
                // This is a query parameter, try to match from query string
                var matchingQueryParam = queryStringParameters.FirstOrDefault(kvp =>
                    string.Equals(kvp.Key, parameter.Name, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(matchingQueryParam.Key))
                {
                    args[i] = ConvertParameterValue(matchingQueryParam.Value, parameter.ParameterType);
                }
            }
        }

        var result = _performMethod.Invoke(null, args);

        if (result is null)
        {
            return null;
        }

        if (result is Task task)
        {
            if (_performMethod.ReturnType != typeof(Task))
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