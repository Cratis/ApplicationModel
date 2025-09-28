// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_QueryPipeline.when_performing;

public class with_valid_query_and_successful_filters : given.a_query_pipeline
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

        _filterResult = QueryResult.Success(correlation_id);
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

    [Fact] void should_return_successful_result() => _result.IsSuccess.ShouldBeTrue();
    [Fact] void should_set_correlation_id_on_result() => _result.CorrelationId.ShouldEqual(correlation_id);
    [Fact] void should_set_query_context_with_correct_name() => _capturedContext.Name.ShouldEqual(_queryName);
    [Fact] void should_set_query_context_with_correct_correlation_id() => _capturedContext.CorrelationId.ShouldEqual(correlation_id);
    [Fact] void should_set_query_context_with_correct_paging() => _capturedContext.Paging.ShouldEqual(_paging);
    [Fact] void should_set_query_context_with_correct_sorting() => _capturedContext.Sorting.ShouldEqual(_sorting);
    [Fact] void should_set_query_context_with_correct_parameters() => _capturedContext.Parameters.ShouldEqual(_parameters);
    [Fact] void should_set_rendered_data_on_result() => _result.Data.ShouldEqual(_rendererResult.Data);
    [Fact] void should_set_paging_info_as_not_paged() => _result.Paging.ShouldEqual(PagingInfo.NotPaged);
    [Fact] void should_call_query_context_manager_set() => query_context_manager.Received(1).Set(Arg.Any<QueryContext>());
}