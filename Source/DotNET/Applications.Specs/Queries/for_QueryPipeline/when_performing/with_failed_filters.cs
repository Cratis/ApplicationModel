// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_QueryPipeline.when_performing;

public class with_failed_filters : given.a_query_pipeline
{
    QueryName _queryName;
    object _parameters;
    Paging _paging;
    Sorting _sorting;
    QueryResult _filterResult;
    QueryResult _result;

    void Establish()
    {
        _queryName = "FilteredQuery";
        _parameters = new { id = 42 };
        _paging = Paging.NotPaged;
        _sorting = Sorting.None;

        _filterResult = new QueryResult
        {
            CorrelationId = correlation_id,
            IsAuthorized = false
        };

        query_performer.Dependencies.Returns(new List<Type>());
        query_performer_providers.TryGetPerformersFor(_queryName, out var _).Returns(callInfo =>
        {
            callInfo[1] = query_performer;
            return true;
        });

        query_filters.OnPerform(Arg.Any<QueryContext>()).Returns(_filterResult);
    }

    async Task Because() => _result = await pipeline.Perform(_queryName, _parameters, _paging, _sorting);

    [Fact] void should_return_filter_result() => _result.ShouldEqual(_filterResult);
    [Fact] void should_not_be_successful() => _result.IsSuccess.ShouldBeFalse();
    [Fact] void should_not_be_authorized() => _result.IsAuthorized.ShouldBeFalse();
    [Fact] void should_not_call_query_performer() => query_performer.DidNotReceiveWithAnyArgs().Perform(Arg.Any<QueryContext>());
    [Fact] void should_not_call_query_renderers() => query_renderers.DidNotReceiveWithAnyArgs().Render(Arg.Any<QueryName>(), Arg.Any<object>());
}