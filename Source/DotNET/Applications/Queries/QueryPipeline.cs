// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents a query pipeline.
/// </summary>
/// <param name="correlationIdAccessor">Accessor for the current correlation ID.</param>
/// <param name="queryContextManager">Manages the current query context.</param>
/// <param name="queryFilters">The query filters.</param>
/// <param name="queryRendererProviders">The query performer providers.</param>
/// <param name="queryRenderers">The query renderers.</param>
/// <param name="serviceProvider">Service provider for resolving dependencies.</param>
public class QueryPipeline(
    ICorrelationIdAccessor correlationIdAccessor,
    IQueryContextManager queryContextManager,
    IQueryFilters queryFilters,
    IQueryPerformerProviders queryRendererProviders,
    IQueryRenderers queryRenderers,
    IServiceProvider serviceProvider) : IQueryPipeline
{
    /// <inheritdoc/>
    public async Task<QueryResult> Perform(QueryName queryName, object parameters, Paging paging, Sorting sorting)
    {
        if (!queryRendererProviders.TryGetPerformersFor(queryName, out var queryRenderer))
        {
            // Return QueryResult - Missing Renderer
            throw new NotImplementedException();
        }

        var dependencies = queryRenderer.Dependencies.Select(serviceProvider.GetRequiredService);
        var correlationId = GetCorrelationId();
        var context = new QueryContext(queryName, correlationId, paging, sorting, parameters, dependencies);
        queryContextManager.Set(context);

        var result = await queryFilters.OnPerform(context);
        var rendererResult = queryRenderers.Render(queryName, result.Data);
        return result;
    }

    CorrelationId GetCorrelationId()
    {
        var correlationId = correlationIdAccessor.Current;
        if (correlationId == CorrelationId.NotSet)
        {
            correlationId = CorrelationId.New();
        }

        return correlationId;
    }
}
