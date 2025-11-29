// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents configuration options for observable query transport mechanisms.
/// </summary>
public class ObservableQueryTransportOptions
{
#if NET10_0_OR_GREATER
    /// <summary>
    /// Gets or sets the preferred transport type.
    /// The system will attempt to use this transport first.
    /// Default: ServerSentEvents.
    /// </summary>
    public TransportType PreferredTransport { get; set; } = TransportType.ServerSentEvents;
#else
    /// <summary>
    /// Gets or sets the preferred transport type.
    /// The system will attempt to use this transport first.
    /// Default: WebSocket.
    /// </summary>
    public TransportType PreferredTransport { get; set; } = TransportType.WebSocket;
#endif

#if NET10_0_OR_GREATER
    /// <summary>
    /// Gets or sets a value indicating whether to enable automatic fallback to the other transport if the preferred one is not available.
    /// When enabled, if PreferredTransport is ServerSentEvents and not available, it will try WebSocket, and vice versa.
    /// Default: true.
    /// </summary>
#else
    /// <summary>
    /// Gets or sets a value indicating whether to enable automatic fallback to the other transport if the preferred one is not available.
    /// Default: true.
    /// </summary>
#endif
    public bool EnableFallback { get; set; } = true;
}
