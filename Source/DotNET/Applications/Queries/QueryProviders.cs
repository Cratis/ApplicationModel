// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Cratis.Types;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents an implementation of <see cref="IQueryProviders"/>.
/// </summary>
/// <param name="queryContextManager"><see cref="IQueryContextManager"/> for managing query contexts.</param>
/// <param name="correlationIdAccessor"><see cref="ICorrelationIdAccessor"/> for getting the current correlation ID.</param>
/// <param name="types"><see cref="ITypes"/> for type discovery.</param>
/// <param name="serviceProvider"><see cref="IServiceProvider"/> for getting instances of query providers.</param>
public class QueryProviders(
    IQueryContextManager queryContextManager,
    ICorrelationIdAccessor correlationIdAccessor,
    ITypes types,
    IServiceProvider serviceProvider) : IQueryProviders
{
    readonly IEnumerable<Type> _queryProviders = types.FindMultiple(typeof(IQueryProviderFor<>));

    /// <inheritdoc/>
    public QueryProviderResult Execute(object query)
    {
        var queryType = query.GetType();
        var queryProviderType = _queryProviders.FirstOrDefault(_ => queryType.IsAssignableTo(_.GetInterface(typeof(IQueryProviderFor<>).Name)!.GetGenericArguments()[0]));
        var queryContext = queryContextManager.Current ?? new QueryContext(correlationIdAccessor.Current, Paging.NotPaged, Sorting.None);
        if (queryProviderType == null)
        {
            return new(queryContext.TotalItems, query);
        }

        var queryProvider = serviceProvider.GetService(queryProviderType);
        var method = queryProviderType.GetMethod(nameof(IQueryProviderFor<object>.Execute))!;
        return (method.Invoke(queryProvider, [query, queryContextManager.Current]) as QueryProviderResult)!;
    }
}