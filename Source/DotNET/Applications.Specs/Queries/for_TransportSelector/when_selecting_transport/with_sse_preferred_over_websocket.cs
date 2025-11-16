// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Cratis.Applications.Queries.for_TransportSelector.when_selecting_transport;

public class with_sse_preferred_over_websocket : given.a_transport_selector
{
    TransportType? _result;

    void Establish()
    {
        _options.PreferredTransports = [TransportType.ServerSentEvents, TransportType.WebSocket];
        _selector = new TransportSelector(Microsoft.Extensions.Options.Options.Create(_options));
        _httpContext = new DefaultHttpContext();
        _webSocketFeature = Substitute.For<IHttpWebSocketFeature>();
        _httpContext.Features.Set(_webSocketFeature);
        _webSocketFeature.IsWebSocketRequest.Returns(true);
        _httpContext.Request.Headers.Upgrade = "websocket";
        _httpContext.Request.Headers.Connection = "Upgrade";
        _httpContext.Request.Headers.SecWebSocketKey = "dGhlIHNhbXBsZSBub25jZQ==";
        _httpContext.Request.Headers.SecWebSocketVersion = "13";
    }

    void Because() => _result = _selector.SelectTransport(_httpContext);

    [Fact] void should_select_server_sent_events() => _result.ShouldEqual(TransportType.ServerSentEvents);
}
