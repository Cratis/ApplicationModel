// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        var args = parameters.Length == 0
            ? null
            : context.Dependencies?.Take(parameters.Length).ToArray();
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
}