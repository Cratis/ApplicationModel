// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a renderer that can render a query.
/// </summary>
public interface IQueryPerformer
{
    /// <summary>
    /// Gets the location the query is at.
    /// </summary>
    /// <remarks>
    /// This is used to determine which renderer should render a given query based on its location.
    /// </remarks>
    IEnumerable<string> Location { get; }

    /// <summary>
    /// Gets the dependencies required by the renderer.
    /// </summary>
    IEnumerable<Type> Dependencies { get; }

    /// <summary>
    /// Renders the given query.
    /// </summary>
    /// <param name="context">The context for the query to render.</param>
    /// <returns>The result of rendering the query.</returns>
    Task<IQueryResult> Perform(QueryContext context);
}
