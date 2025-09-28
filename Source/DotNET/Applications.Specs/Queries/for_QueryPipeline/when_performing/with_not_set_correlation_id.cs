// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Queries.for_QueryPipeline.when_performing;

public class with_not_set_correlation_id : given.a_query_pipeline
{
    QueryName _queryName;
    object _parameters;
    Paging _paging;
    Sorting _sorting;
    QueryResult _filterResult;
    object _queryData;
    QueryRendererResult _rendererResult;
    QueryResult _result;
    QueryContext _capturedContext;

    void Establish()
    {
        _queryName = "TestQuery";
        _parameters = new { id = 42 };
        _paging = Paging.NotPaged;
        _sorting = Sorting.None;

        correlation_id_accessor.Current.Returns(CorrelationId.NotSet);

        _filterResult = QueryResult.Success(CorrelationId.NotSet);
        _queryData = new { name = "Test Data" };
        _rendererResult = new QueryRendererResult(100, new { renderedData = "Rendered Test Data" });

        query_performer.Dependencies.Returns(new List<Type>());
        query_performer_providers.TryGetPerformersFor(_queryName, out var _).Returns(callInfo =>
        {
            callInfo[1] = query_performer;
            return true;
        });

        query_filters.OnPerform(Arg.Do<QueryContext>(ctx => _capturedContext = ctx)).Returns(_filterResult);
        query_performer.Perform(Arg.Any<QueryContext>()).Returns(Task.FromResult<object?>(_queryData));
        query_renderers.Render(_queryName, _queryData).Returns(_rendererResult);
    }

    async Task Because() => _result = await pipeline.Perform(_queryName, _parameters, _paging, _sorting);

    [Fact] void should_generate_new_correlation_id_for_context() => _capturedContext.CorrelationId.ShouldNotEqual(CorrelationId.NotSet);
    [Fact] void should_use_generated_correlation_id_consistently() => _capturedContext.CorrelationId.Value.ShouldNotEqual(Guid.Empty);
}