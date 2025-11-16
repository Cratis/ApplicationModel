// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_TransportSelector.when_selecting_transport;

public class with_websocket_request : given.a_transport_selector
{
    TransportType? _result;

    void Establish()
    {
        _options.PreferredTransport = TransportType.WebSocket;
        _selector = new TransportSelector(Microsoft.Extensions.Options.Options.Create(_options));
        _webSocketFeature.IsWebSocketRequest.Returns(true);
        _httpContext.Request.Headers.Upgrade = "websocket";
        _httpContext.Request.Headers.Connection = "Upgrade";
        _httpContext.Request.Headers.SecWebSocketKey = "dGhlIHNhbXBsZSBub25jZQ==";
        _httpContext.Request.Headers.SecWebSocketVersion = "13";
    }

    void Because() => _result = _selector.SelectTransport(_httpContext);

    [Fact] void should_select_websocket() => _result.ShouldEqual(TransportType.WebSocket);
}
