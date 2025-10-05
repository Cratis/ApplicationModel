// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Commands.Filters.for_AuthorizationFilter.when_executing;

public class with_command_with_authorize_and_unauthenticated_user : given.an_authorization_filter
{
    CommandResult _result;

    void Establish()
    {
        SetupCommandWithType(typeof(given.CommandWithAuthorize), new given.CommandWithAuthorize());
        SetupUnauthenticatedUser();
    }

    async Task Because() => _result = await _filter.OnExecution(_context);

    [Fact] void should_not_be_successful() => _result.IsSuccess.ShouldBeFalse();
    [Fact] void should_have_correlation_id() => _result.CorrelationId.ShouldEqual(_correlationId);
    [Fact] void should_not_be_authorized() => _result.IsAuthorized.ShouldBeFalse();
    [Fact] void should_be_valid() => _result.IsValid.ShouldBeTrue();
    [Fact] void should_not_have_exceptions() => _result.HasExceptions.ShouldBeFalse();
}