// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_TransportSelector.when_selecting_transport;

public class with_regular_http_request : given.a_transport_selector
{
    TransportType? _result;

    void Because() => _result = _selector.SelectTransport(_httpContext);

    [Fact] void should_select_server_sent_events_as_fallback() => _result.ShouldEqual(TransportType.ServerSentEvents);
}
