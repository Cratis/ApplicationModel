// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Validation;
using Cratis.Execution;

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
