// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Cratis.Execution;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents an implementation of <see cref="QueryContext"/>.
/// </summary>
[Singleton]
public class QueryContextManager : IQueryContextManager
{
    static readonly AsyncLocal<QueryContext> _context = new();

    /// <inheritdoc/>
    public QueryContext Current => _context.Value ?? new QueryContext(CorrelationId.New(), Paging.NotPaged, Sorting.None);

    /// <inheritdoc/>
    public void Set(QueryContext context) => _context.Value = context;
}
