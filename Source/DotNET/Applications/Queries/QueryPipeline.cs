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
/// <param name="queryPerformerProviders">The query performer providers.</param>
/// <param name="queryRenderers">The query renderers.</param>
/// <param name="serviceProvider">Service provider for resolving dependencies.</param>
public class QueryPipeline(
    ICorrelationIdAccessor correlationIdAccessor,
    IQueryContextManager queryContextManager,
    IQueryFilters queryFilters,
    IQueryPerformerProviders queryPerformerProviders,
    IQueryRenderers queryRenderers,
    IServiceProvider serviceProvider) : IQueryPipeline
{
    /// <inheritdoc/>
    public async Task<QueryResult> Perform(QueryName queryName, object parameters, Paging paging, Sorting sorting)
    {
        var result = QueryResult.Success(correlationIdAccessor.Current);
        try
        {
            if (!queryPerformerProviders.TryGetPerformersFor(queryName, out var queryPerformer))
            {
                return QueryResult.MissingPerformer(queryName);
            }

            var dependencies = queryPerformer.Dependencies.Select(serviceProvider.GetRequiredService);
            var correlationId = GetCorrelationId();
            var context = new QueryContext(queryName, correlationId, paging, sorting, parameters, dependencies);
            queryContextManager.Set(context);

            result = await queryFilters.OnPerform(context);
            if (!result.IsSuccess)
            {
                return result;
            }
            var data = await queryPerformer.Perform(context);
            if (data is null)
            {
                return result;
            }
            var rendererResult = queryRenderers.Render(queryName, data);
            if (rendererResult is null)
            {
                return QueryResult.Error("No renderer result");
            }
            result.Data = rendererResult.Data;
            result.Paging = context.Paging == Paging.NotPaged ? PagingInfo.NotPaged : new PagingInfo(
                        context.Paging.Page,
                        context.Paging.Size,
                        rendererResult.TotalItems);

            return result;
        }
        catch (Exception ex)
        {
            result.MergeWith(QueryResult.Error(ex));
        }

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
