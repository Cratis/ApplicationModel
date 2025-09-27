// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Cratis.Types;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents an implementation of <see cref="IQueryPerformerProviders"/>.
/// </summary>
public class QueryPerformerProviders : IQueryPerformerProviders
{
    readonly IDictionary<QueryName, IQueryPerformer> _performers;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryPerformerProviders"/> class.
    /// </summary>
    /// <param name="providers">Instances of <see cref="IQueryPerformerProvider"/> to use for providing query performers.</param>
    public QueryPerformerProviders(IInstancesOf<IQueryPerformerProvider> providers)
    {
        var performers = providers.SelectMany(p => p.Performers);
        MultipleQueryPerformersForSameQueryType.ThrowIfDuplicates(performers);
        _performers = performers.ToDictionary(p => p.Name, p => p);
    }

    /// <inheritdoc/>
    public IEnumerable<IQueryPerformer> Performers => _performers.Values;

    /// <inheritdoc/>
    public bool TryGetPerformersFor(QueryName query, [NotNullWhen(true)] out IQueryPerformer? performer) =>
        _performers.TryGetValue(query, out performer);
}
