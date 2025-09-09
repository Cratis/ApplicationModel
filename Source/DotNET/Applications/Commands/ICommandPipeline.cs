// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Commands;

/// <summary>
/// Defines a system can execute commands.
/// </summary>
public interface ICommandPipeline
{
    /// <summary>
    /// Executes the given command.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <returns>A <see cref="CommandResult"/> representing the result of executing the command.</returns>
    Task<CommandResult> Execute(object command);
}
