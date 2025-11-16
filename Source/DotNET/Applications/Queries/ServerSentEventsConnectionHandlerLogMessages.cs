// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NET10_0_OR_GREATER
using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Queries;

/// <summary>
/// Log messages for <see cref="ServerSentEventsConnectionHandler"/>.
/// </summary>
internal static partial class ServerSentEventsConnectionHandlerLogMessages
{
    [LoggerMessage(0, LogLevel.Trace, "SSE connection cancelled")]
    internal static partial void SseConnectionCancelled(this ILogger<ServerSentEventsConnectionHandler> logger);

    [LoggerMessage(1, LogLevel.Error, "Error streaming SSE query results")]
    internal static partial void SseStreamingError(this ILogger<ServerSentEventsConnectionHandler> logger, Exception ex);
}
#endif
