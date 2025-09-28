// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Queries.for_QueryPipeline.when_performing;

public class with_null_renderer_result : given.a_query_pipeline
{
    QueryName _queryName;
    object _parameters;
    Paging _paging;
    Sorting _sorting;
    QueryResult _filterResult;
    object _queryData;
    QueryResult _result;

    void Establish()
    {
        _queryName = "QueryWithNullRenderer";
        _parameters = new { id = 42 };
        _paging = Paging.NotPaged;
        _sorting = Sorting.None;

        _filterResult = QueryResult.Success(correlation_id);
        _queryData = new { name = "Test Data" };

        query_performer.Dependencies.Returns(new List<Type>());
        query_performer_providers.TryGetPerformersFor(_queryName, out var _).Returns(callInfo =>
        {
            callInfo[1] = query_performer;
            return true;
        });

        query_filters.OnPerform(Arg.Any<QueryContext>()).Returns(_filterResult);
        query_performer.Perform(Arg.Any<QueryContext>()).Returns(Task.FromResult<object?>(_queryData));
        query_renderers.Render(_queryName, _queryData).Returns((QueryRendererResult?)null);
    }

    async Task Because() => _result = await pipeline.Perform(_queryName, _parameters, _paging, _sorting);

    [Fact] void should_return_unsuccessful_result() => _result.IsSuccess.ShouldBeFalse();
    [Fact] void should_have_error_message_about_no_renderer_result() => _result.ExceptionMessages.ShouldContain("No renderer result");
    [Fact] void should_have_empty_correlation_id() => _result.CorrelationId.ShouldEqual(new CorrelationId(Guid.Empty));
}