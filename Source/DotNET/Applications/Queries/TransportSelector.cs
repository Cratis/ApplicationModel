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
        // Try preferred transport first
        if (IsTransportSupported(httpContext, _options.PreferredTransport))
        {
            return _options.PreferredTransport;
        }

        // If fallback is enabled, try the other transport
        if (_options.EnableFallback)
        {
#if NET10_0_OR_GREATER
            var fallbackTransport = _options.PreferredTransport == TransportType.WebSocket
                ? TransportType.ServerSentEvents
                : TransportType.WebSocket;
#else
            const TransportType fallbackTransport = TransportType.WebSocket;
#endif

            if (IsTransportSupported(httpContext, fallbackTransport))
            {
                return fallbackTransport;
            }
        }

        return null;
    }

    static bool IsTransportSupported(HttpContext httpContext, TransportType transport)
    {
        return transport switch
        {
            TransportType.WebSocket => httpContext.WebSockets.IsWebSocketRequest,
#if NET10_0_OR_GREATER
            TransportType.ServerSentEvents => IsServerSentEventsRequest(httpContext),
#endif
            _ => false
        };
    }

#if NET10_0_OR_GREATER
    static bool IsServerSentEventsRequest(HttpContext _)
    {
        // SSE is always technically supported for any HTTP request
        // Explicit preference is handled by the preference order in options
        // Only reject SSE if the client explicitly only wants WebSocket (no fallback)
        return true;
    }
#endif
}
