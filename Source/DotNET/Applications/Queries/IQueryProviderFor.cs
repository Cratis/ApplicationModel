// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a paging adorner for a query.
/// </summary>
/// <typeparam name="TQueryType">Type of query.</typeparam>
public interface IQueryProviderFor<TQueryType>
{
    /// <summary>
    /// Adorn the query with paging.
    /// </summary>
    /// <param name="query">Query to adorn.</param>
    /// <param name="queryContext">Context of the query.</param>
    /// <returns>The adorned query.</returns>
    QueryProviderResult Execute(TQueryType query, QueryContext queryContext);
}
