// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Cratis.Applications.Authorization;
using Cratis.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Queries.ModelBound;

/// <summary>
/// Represents a provider for query performers that are model bound.
/// </summary>
public class QueryPerformerProvider : IQueryPerformerProvider
{
    readonly Dictionary<QueryName, IQueryPerformer> _performers;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryPerformerProvider"/> class.
    /// </summary>
    /// <param name="types">The types to scan for read models.</param>
    /// <param name="serviceProviderIsService">Service to determine if a type is registered as a service.</param>
    /// <param name="authorizationEvaluator">The authorization evaluator.</param>
    public QueryPerformerProvider(ITypes types, IServiceProviderIsService serviceProviderIsService, IAuthorizationEvaluator authorizationEvaluator)
    {
        var readModelTypes = types.All.Where(t => t.IsReadModel());
        _performers = readModelTypes
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.IsValidQueryFor(t))
                .Select(m => new ModelBoundQueryPerformer(t, m, serviceProviderIsService, authorizationEvaluator)))
            .ToDictionary(p => p.Name, p => (IQueryPerformer)p);
    }

    /// <inheritdoc/>
    public IEnumerable<IQueryPerformer> Performers => _performers.Values;

    /// <inheritdoc/>
    public bool TryGetPerformerFor(QueryName query, [NotNullWhen(true)] out IQueryPerformer? performer) =>
        _performers.TryGetValue(query, out performer);
}
