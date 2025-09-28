// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_QueryPipeline.when_performing;

public class with_null_query_data : given.a_query_pipeline
{
    QueryName _queryName;
    object _parameters;
    Paging _paging;
    Sorting _sorting;
    QueryResult _filterResult;
    QueryResult _result;

    void Establish()
    {
        _queryName = "QueryWithNullData";
        _parameters = new { id = 42 };
        _paging = Paging.NotPaged;
        _sorting = Sorting.None;

        _filterResult = QueryResult.Success(correlation_id);

        query_performer.Dependencies.Returns(new List<Type>());
        query_performer_providers.TryGetPerformersFor(_queryName, out var _).Returns(callInfo =>
        {
            callInfo[1] = query_performer;
            return true;
        });

        query_filters.OnPerform(Arg.Any<QueryContext>()).Returns(_filterResult);
        query_performer.Perform(Arg.Any<QueryContext>()).Returns(Task.FromResult<object?>(null));
    }

    async Task Because() => _result = await pipeline.Perform(_queryName, _parameters, _paging, _sorting);

    [Fact] void should_return_successful_result() => _result.IsSuccess.ShouldBeTrue();
    [Fact] void should_have_correlation_id_from_filter_result() => _result.CorrelationId.ShouldEqual(correlation_id);
    [Fact] void should_not_call_query_renderers() => query_renderers.DidNotReceiveWithAnyArgs().Render(Arg.Any<QueryName>(), Arg.Any<object>());
    [Fact] void should_have_default_data() => _result.Data.ShouldBeNull();
}