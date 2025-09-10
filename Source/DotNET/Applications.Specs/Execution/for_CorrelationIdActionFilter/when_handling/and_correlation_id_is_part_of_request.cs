// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.Extensions.Primitives;

namespace Cratis.Applications.Execution.for_CorrelationIdActionFilter.when_handling;

public class and_correlation_id_is_part_of_request : given.a_correlation_id_action_filter
{
    string _correlationIdAsString;
    CorrelationId _correlationId;

    void Establish()
    {
        _correlationId = CorrelationId.New();
        _correlationIdAsString = _correlationId.ToString();
        _headers[Constants.DefaultCorrelationIdHeader].Returns(new StringValues(_correlationIdAsString));
    }

    Task Because() => _correlationIdActionFilter.OnActionExecutionAsync(_actionExecutingContext, _next);

    [Fact] void should_set_current_correlation_id() => _correlationIdModifier.Received(1).Modify(_correlationId);
    [Fact] void should_set_correlation_id_in_http_context_items() => _actionExecutingContext.HttpContext.Items[Constants.CorrelationIdItemKey].ShouldEqual(_correlationId);
}
