// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents an implementation of <see cref="ITransportSelector"/>.
/// </summary>
/// <param name="options">The <see cref="ObservableQueryTransportOptions"/>.</param>
[Singleton]
public class TransportSelector(IOptions<ObservableQueryTransportOptions> options) : ITransportSelector
{
    readonly ObservableQueryTransportOptions _options = options.Value;

    /// <inheritdoc/>
    public TransportType? SelectTransport(HttpContext httpContext)
    {
        foreach (var transport in _options.PreferredTransports)
        {
            if (IsTransportSupported(httpContext, transport))
            {
                return transport;
            }
        }

        return null;
    }

    static bool IsTransportSupported(HttpContext httpContext, TransportType transport)
    {
        return transport switch
        {
            TransportType.WebSocket => httpContext.WebSockets.IsWebSocketRequest,
            TransportType.ServerSentEvents => IsServerSentEventsRequest(httpContext),
            _ => false
        };
    }

    static bool IsServerSentEventsRequest(HttpContext httpContext)
    {
        // SSE is supported when Accept header includes text/event-stream
        // or when there's no specific WebSocket upgrade request
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        return acceptHeader.Contains("text/event-stream", StringComparison.OrdinalIgnoreCase) ||
               !httpContext.WebSockets.IsWebSocketRequest;
    }
}
