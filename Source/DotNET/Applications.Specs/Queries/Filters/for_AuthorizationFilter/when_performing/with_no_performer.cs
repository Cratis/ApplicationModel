// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.Filters.for_AuthorizationFilter.when_performing;

public class with_no_performer : given.an_authorization_filter
{
    QueryResult _result;

    void Establish()
    {
        SetupQueryWithoutPerformer();
        SetupAuthenticatedUser();
    }

    async Task Because() => _result = await _filter.OnPerform(_context);

    [Fact] void should_return_successful_result() => _result.IsSuccess.ShouldBeTrue();
    [Fact] void should_have_correlation_id() => _result.CorrelationId.ShouldEqual(_correlationId);
    [Fact] void should_be_authorized() => _result.IsAuthorized.ShouldBeTrue();
    [Fact] void should_be_valid() => _result.IsValid.ShouldBeTrue();
    [Fact] void should_not_have_exceptions() => _result.HasExceptions.ShouldBeFalse();
}