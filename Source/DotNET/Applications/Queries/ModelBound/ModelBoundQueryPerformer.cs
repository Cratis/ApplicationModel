// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Reflection;
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
        Type = readModelType;
        Name = performMethod.Name;
        FullyQualifiedName = $"{readModelType.FullName}.{performMethod.Name}";
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
    public FullyQualifiedQueryName FullyQualifiedName { get; }

    /// <inheritdoc/>
    public Type Type { get; }

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
        var dependencies = context.Dependencies?.ToArray() ?? [];
        var queryStringParameters = context.Arguments ?? QueryArguments.Empty;
        var args = GetMethodArguments(parameters, dependencies, queryStringParameters);

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

    object?[] GetMethodArguments(ParameterInfo[] parameters, object[] dependencies, QueryArguments queryStringParameters)
    {
        var dependencyIndex = 0;
        var args = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

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
                var matchingQueryParam = queryStringParameters.FirstOrDefault(kvp =>
                    string.Equals(kvp.Key, parameter.Name, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(matchingQueryParam.Key))
                {
                    args[i] = matchingQueryParam.Value.ConvertTo(parameter.ParameterType);
                }
            }
        }

        ValidateArguments(parameters, args);
        return args;
    }

    void ValidateArguments(ParameterInfo[] parameters, object?[] args)
    {
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var arg = args[i];

            if (arg is null && !parameter.ParameterType.IsNullable())
            {
                throw new MissingArgumentForQuery(parameter.Name ?? "unknown", parameter.ParameterType, FullyQualifiedName);
            }
        }
    }
}