// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Validation;
using Cratis.Execution;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents the result coming from performing a query.
/// </summary>
/// <typeparam name="T">Type of the data returned.</typeparam>
public class QueryResult<T>
{
    /// <summary>
    /// Represents a successful command result.
    /// </summary>
    public static readonly QueryResult<T> Success = new();

    /// <summary>
    /// Gets or inits the <see cref="PagingInfo"/> for the query.
    /// </summary>
    public PagingInfo Paging { get; set; } = PagingInfo.NotPaged;

    /// <summary>
    /// Gets the <see cref="CorrelationId"/> associated with the command.
    /// </summary>
    public CorrelationId CorrelationId { get; init; } = new(Guid.Empty.ToString());

    /// <summary>
    /// The data returned.
    /// </summary>
    public T Data { get; set; } = default!;

    /// <summary>
    /// Gets whether or not the query executed successfully.
    /// </summary>
    public bool IsSuccess => IsAuthorized && IsValid && !HasExceptions;

    /// <summary>
    /// Gets whether or not the query was authorized to execute.
    /// </summary>
    public bool IsAuthorized { get; init; } = true;

    /// <summary>
    /// Gets whether or not the query is valid.
    /// </summary>
    public bool IsValid => !ValidationResults.Any();

    /// <summary>
    /// Gets whether or not there are any exceptions that occurred.
    /// </summary>
    public bool HasExceptions => ExceptionMessages.Any();

    /// <summary>
    /// Gets any validation errors. If this collection is empty, there are errors.
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
