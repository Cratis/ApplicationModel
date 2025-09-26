// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Validation;
using Cratis.Execution;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents the result coming from performing a query.
/// </summary>
/// <typeparam name="T">Type of the data returned.</typeparam>
public class QueryResult<T> : IQueryResult
{
    /// <summary>
    /// Represents a successful command result.
    /// </summary>
    public static readonly QueryResult<T> Success = new();

    /// <inheritdoc/>
    public PagingInfo Paging { get; set; } = PagingInfo.NotPaged;

    /// <inheritdoc/>
    public CorrelationId CorrelationId { get; init; } = new(Guid.Empty);

    /// <summary>
    /// The data returned.
    /// </summary>
    public T Data { get; set; } = default!;

    /// <inheritdoc/>
    object IQueryResult.Data { get => Data!; set => Data = (T)value; }

    /// <inheritdoc/>
    public bool IsSuccess => IsAuthorized && IsValid && !HasExceptions;

    /// <inheritdoc/>
    public bool IsAuthorized { get; init; } = true;

    /// <inheritdoc/>
    public bool IsValid => !ValidationResults.Any();

    /// <inheritdoc/>
    public bool HasExceptions => ExceptionMessages.Any();

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> ValidationResults { get; init; } = [];

    /// <inheritdoc/>
    public IEnumerable<string> ExceptionMessages { get; set; } = [];

    /// <inheritdoc/>
    public string ExceptionStackTrace { get; init; } = string.Empty;
}
