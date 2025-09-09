// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Commands;

/// <summary>
/// Defines a handler for processing command response values.
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Gets the location the command is at.
    /// </summary>
    /// <remarks>
    /// This is used to determine which handler should handle a given command based on its location.
    /// </remarks>
    IEnumerable<string> Location { get; }

    /// <summary>
    /// Gets the type of command the handler can handle.
    /// </summary>
    Type CommandType { get; }

    /// <summary>
    /// Gets the dependencies required by the handler.
    /// </summary>
    IEnumerable<Type> Dependencies { get; }

    /// <summary>
    /// Handles the given command.
    /// </summary>
    /// <param name="commandContext">The context for the command being handled.</param>
    /// <returns>Response from handling the command.</returns>
    Task<object?> Handle(CommandContext commandContext);
}
