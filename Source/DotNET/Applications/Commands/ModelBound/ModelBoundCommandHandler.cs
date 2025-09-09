// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Applications.Commands.ModelBound;

/// <summary>
/// Represents an implementation of <see cref="ICommandHandler"/> for model-bound commands.
/// </summary>
/// <param name="commandType">The type of command the handler can handle.</param>
/// <param name="handleMethod">The method info of the handle method.</param>
public class ModelBoundCommandHandler(Type commandType, MethodInfo handleMethod) : ICommandHandler
{
    /// <inheritdoc/>
    public IEnumerable<string> Location { get; } = commandType.Namespace?.Split('.') ?? [];

    /// <inheritdoc/>
    public Type CommandType => commandType;

    /// <inheritdoc/>
    public IEnumerable<Type> Dependencies { get; } = handleMethod.GetParameters().Select(p => p.ParameterType);

    /// <inheritdoc/>
    public async Task<object?> Handle(CommandContext commandContext)
    {
        var result = handleMethod.Invoke(commandContext.Command, [.. commandContext.Dependencies]);
        if (result is not null && result is Task taskResult)
        {
            if (taskResult.GetType().IsGenericType && taskResult.GetType().GetGenericTypeDefinition() == typeof(Task<>))
            {
                return (Task<object?>)taskResult;
            }

            await taskResult;
            return null;
        }

        return Task.FromResult(result);
    }
}
