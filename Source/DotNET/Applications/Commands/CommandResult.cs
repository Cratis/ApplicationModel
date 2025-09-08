// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cratis.Applications.Validation;
using Cratis.Execution;
using OneOf;

namespace Cratis.Applications.Commands;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Represents the result coming from executing a command.
/// </summary>
public class CommandResult
{
    /// <summary>
    /// Gets the <see cref="CorrelationId"/> associated with the command.
    /// </summary>
    public CorrelationId CorrelationId { get; init; } = new(Guid.Empty);

    /// <summary>
    /// Gets whether or not the command executed successfully.
    /// </summary>
    public bool IsSuccess => IsAuthorized && IsValid && !HasExceptions;

    /// <summary>
    /// Gets whether or not the command was authorized to execute.
    /// </summary>
    public bool IsAuthorized { get; init; } = true;

    /// <summary>
    /// Gets whether or not the command is valid.
    /// </summary>
    public bool IsValid => !ValidationResults.Any();

    /// <summary>
    /// Gets whether or not there are any exceptions that occurred.
    /// </summary>
    public bool HasExceptions => ExceptionMessages.Any();

    /// <summary>
    /// Gets any validation result.
    /// </summary>
    public IEnumerable<ValidationResult> ValidationResults { get; init; } = [];

    /// <summary>
    /// Gets any exception messages that might have occurred.
    /// </summary>
    public IEnumerable<string> ExceptionMessages { get; init; } = [];

    /// <summary>
    /// Gets the stack trace if there was an exception.
    /// </summary>
    public string ExceptionStackTrace { get; init; } = string.Empty;

    /// <summary>
    /// Creates a new <see cref="CommandResult"/> representing a missing handler.
    /// </summary>
    /// <param name="type">The type of command that is missing a handler.</param>
    /// <returns>A <see cref="CommandResult"/>.</returns>
    public static CommandResult MissingHandler(Type type) => new() { ExceptionMessages = [$"No handler found for command of type {type}"] };
}

/// <summary>
/// Represents the result coming from executing a command with a response.
/// </summary>
/// <typeparam name="T">Type of the data returned.</typeparam>
public class CommandResult<T> : CommandResult
{
    /// <summary>
    /// Represents a successful command result.
    /// </summary>
    public static readonly CommandResult<T> Success = new();

    /// <summary>
    /// Optional response object. Controller actions representing a command can optionally return a response as any type, this is where it would be.
    /// </summary>
    public T? Response { get; init; }
}

public interface ICommandResponseValueHandler<TValue>
{
    Task<CommandResult> Handle(TValue value);
}

public interface ICommandResponseValueHandlers
{
    Task<CommandResult> Handle(object value);
}

public interface ICommandPipeline
{
    Task<CommandResult> Execute(object command);
}

public interface ICommandHandler
{
    Task<OneOf<object, ITuple>> Handle(object command);
}

public interface ICommandHandlerProvider
{
    bool TryGetHandlerFor(object command, [NotNullWhen(true)] out ICommandHandler? handler);
}

public interface ICommandHandlerProviders
{
    bool TryGetHandlerFor(object command, [NotNullWhen(true)] out ICommandHandler? handler);
}

public class CommandPipeline(
    ICommandHandlerProviders handlerProviders,
    ICommandResponseValueHandlers valueHandlers) : ICommandPipeline
{
    /// <inheritdoc/>
    public async Task<CommandResult> Execute(object command)
    {
        handlerProviders.TryGetHandlerFor(command, out var commandHandler);
        if (commandHandler is null)
        {
            return CommandResult.MissingHandler(command.GetType());
        }

        var result = await commandHandler.Handle(command);

        // If the first value is not a tuple
        //    If the first value is a OneOf
        //        Loop through each value in the OneOf and handle the value
        // Else (it is a tuple)
        //    Use the first value in the tuple as the response for the command
        //    Loop through the rest of the values in the tuple and handle each value

        // result = await valueHandlers.Handle(result);
        throw new NotImplementedException();
    }
}
