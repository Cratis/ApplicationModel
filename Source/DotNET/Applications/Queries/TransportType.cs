// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines the available transport types for observable query subscriptions.
/// </summary>
public enum TransportType
{
    /// <summary>
    /// WebSocket transport.
    /// </summary>
    WebSocket = 0,

    /// <summary>
    /// Server-Sent Events (SSE) transport.
    /// </summary>
    ServerSentEvents = 1
}
