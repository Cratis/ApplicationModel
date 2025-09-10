// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.Extensions.Primitives;

namespace Cratis.Applications.Execution.for_CorrelationIdActionFilter.when_handling;

public class and_correlation_id_is_part_of_request_with_null_string : given.a_correlation_id_action_filter
{
    string _correlationIdAsString;

    void Establish()
    {
        _correlationIdAsString = null!;
        _headers[Constants.DefaultCorrelationIdHeader].Returns(new StringValues(_correlationIdAsString));
    }

    Task Because() => _correlationIdActionFilter.OnActionExecutionAsync(_actionExecutingContext, _next);

    [Fact] void should_not_set_an_empty_correlation_id() => _currentCorrelationId.ShouldNotEqual(CorrelationId.NotSet);
    [Fact] void should_set_current_correlation_id() => _correlationIdModifier.Received(1).Modify(_currentCorrelationId);
    [Fact] void should_set_correlation_id_in_http_context_items() => _actionExecutingContext.HttpContext.Items[Constants.CorrelationIdItemKey].ShouldEqual(_currentCorrelationId);
}
