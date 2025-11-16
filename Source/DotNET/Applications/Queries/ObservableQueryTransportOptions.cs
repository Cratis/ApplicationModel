// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents configuration options for observable query transport mechanisms.
/// </summary>
public class ObservableQueryTransportOptions
{
    /// <summary>
    /// Gets or sets the preferred transport types in order of preference.
    /// The system will attempt to use transports in the order specified.
    /// Default: [WebSocket, ServerSentEvents].
    /// </summary>
    public IList<TransportType> PreferredTransports { get; set; } = [TransportType.WebSocket, TransportType.ServerSentEvents];

    /// <summary>
    /// Gets or sets a value indicating whether to enable automatic fallback to the next transport if the preferred one fails.
    /// Default: true.
    /// </summary>
    public bool EnableFallback { get; set; } = true;
}
