// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Queries;

/// <summary>
/// Log messages for <see cref="WebSocketQueryHandler"/>.
/// </summary>
internal static partial class WebSocketQueryHandlerLogMessages
{
    [LoggerMessage(0, LogLevel.Trace, "Controller {Controller} with action {Action} returns a client observable")]
    internal static partial void ClientObservableReturnValue(this ILogger<WebSocketQueryHandler> logger, string controller, string action);

    [LoggerMessage(1, LogLevel.Trace, "Request is WebSocket")]
    internal static partial void RequestIsWebSocket(this ILogger<WebSocketQueryHandler> logger);

    [LoggerMessage(2, LogLevel.Trace, "Request is regular HTTP")]
    internal static partial void RequestIsHttp(this ILogger<WebSocketQueryHandler> logger);

    [LoggerMessage(3, LogLevel.Trace, "Controller {Controller} with action {Action} returns a client enumerable")]
    internal static partial void AsyncEnumerableReturnValue(this ILogger<WebSocketQueryHandler> logger, string controller, string action);
}