// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a system can execute queries.
/// </summary>
public interface IQueryPipeline
{
    /// <summary>
    /// Performs the given query.
    /// </summary>
    /// <param name="queryName">The name of the query to perform.</param>
    /// <param name="parameters">The parameters for the query.</param>
    /// <param name="paging">The paging to apply to the query.</param>
    /// <param name="sorting">The sorting to apply to the query.</param>
    /// <returns>A <see cref="IQueryResult"/> representing the result of executing the command.</returns>
    Task<IQueryResult> Perform(QueryName queryName, object parameters, Paging paging, Sorting sorting);
}
