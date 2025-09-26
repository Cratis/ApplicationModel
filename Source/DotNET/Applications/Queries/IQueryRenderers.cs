// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a system that can execute queries.
/// </summary>
public interface IQueryRenderers
{
    /// <summary>
    /// Render a query.
    /// </summary>
    /// <param name="queryName">Name of the query.</param>
    /// <param name="query">Query to render.</param>
    /// <returns>Result.</returns>
    QueryRendererResult Render(QueryName queryName, object query);
}
