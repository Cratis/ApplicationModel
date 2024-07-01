// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a system that can execute queries.
/// </summary>
public interface IQueryProviders
{
    /// <summary>
    /// Execute a query.
    /// </summary>
    /// <param name="query">Query to execute.</param>
    /// <returns>Result.</returns>
    QueryProviderResult Execute(object query);
}
