// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.Filters.for_AuthorizationFilter.when_performing;

public class and_authorization_fails_with_performer : given.an_authorization_filter
{
    QueryResult _result;
    FullyQualifiedQueryName _queryName;

    void Establish()
    {
        _queryName = new FullyQualifiedQueryName("TestQuery");
        _queryPerformer.Type.Returns(typeof(object));
        _queryPerformerProviders.TryGetPerformersFor(_queryName, out var _).Returns(callInfo =>
        {
            callInfo[1] = _queryPerformer;
            return true;
        });

        _context = new QueryContext(_queryName, _correlationId, Paging.NotPaged, Sorting.None, null, []);
        _authorizationHelper.IsAuthorized(typeof(object)).Returns(false);
    }

    async Task Because() => _result = await _filter.OnPerform(_context);

    [Fact] void should_call_authorization_helper_with_query_type() => _authorizationHelper.Received(1).IsAuthorized(typeof(object));
    [Fact] void should_not_be_successful() => _result.IsSuccess.ShouldBeFalse();
    [Fact] void should_have_correlation_id() => _result.CorrelationId.ShouldEqual(_correlationId);
    [Fact] void should_not_be_authorized() => _result.IsAuthorized.ShouldBeFalse();
    [Fact] void should_be_valid() => _result.IsValid.ShouldBeTrue();
    [Fact] void should_not_have_exceptions() => _result.HasExceptions.ShouldBeFalse();
}