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
        var parameters = handleMethod.GetParameters();
        var args = parameters.Length == 0
            ? null
            : commandContext.Dependencies.Take(parameters.Length).ToArray();
        var result = handleMethod.Invoke(commandContext.Command, args);

        if (result is null)
        {
            return null;
        }

        if (result is Task task)
        {
            await task;

            return task.GetType().GetProperty(nameof(Task<object>.Result)) is { } resultProperty
                ? resultProperty.GetValue(task)
                : null;
        }

        return result;
    }
}
