// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Validation;
using Cratis.Execution;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents the result of a query.
/// </summary>
public interface IQueryResult
{
    /// <summary>
    /// Gets or inits the <see cref="PagingInfo"/> for the query.
    /// </summary>
    PagingInfo Paging { get; set; }

    /// <summary>
    /// Gets the <see cref="CorrelationId"/> associated with the command.
    /// </summary>
    CorrelationId CorrelationId { get; init; }

    /// <summary>
    /// The data returned.
    /// </summary>
    object Data { get; set; }

    /// <summary>
    /// Gets whether or not the query executed successfully.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets whether or not the query was authorized to execute.
    /// </summary>
    bool IsAuthorized { get; }

    /// <summary>
    /// Gets whether or not the query is valid.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets whether or not there are any exceptions that occurred.
    /// </summary>
    bool HasExceptions { get; }

    /// <summary>
    /// Gets any validation errors. If this collection is empty, there are errors.
    /// </summary>
    IEnumerable<ValidationResult> ValidationResults { get; }

    /// <summary>
    /// Gets any exception messages that might have occurred.
    /// </summary>
    IEnumerable<string> ExceptionMessages { get; }

    /// <summary>
    /// Gets the stack trace if there was an exception.
    /// </summary>
    string ExceptionStackTrace { get; }
}
