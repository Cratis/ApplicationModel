// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines the context for a query.
/// </summary>
/// <param name="Name">The name of the query.</param>
/// <param name="CorrelationId">The <see cref="CorrelationId"/> for the query.</param>
/// <param name="Paging">The <see cref="Paging"/> information.</param>
/// <param name="Sorting">The <see cref="Sorting"/> information.</param>
/// <param name="Parameters">Optional parameters for the query.</param>
/// <param name="Dependencies">Optional dependencies required to handle the query.</param>
public record QueryContext(QueryName Name, CorrelationId CorrelationId, Paging Paging, Sorting Sorting, IDictionary<string, object>? Parameters = null, IEnumerable<object>? Dependencies = null)
{
    /// <summary>
    /// Represents a query context that is not set.
    /// </summary>
    public static readonly QueryContext NotSet = new("[NotSet]", CorrelationId.NotSet, Paging.NotPaged, Sorting.None);

    /// <summary>
    /// Gets or sets the total number of items in the query.
    /// </summary>
    public int TotalItems { get; set; }
}
