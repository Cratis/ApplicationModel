// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Queries;

/// <summary>
/// Log messages for <see cref="QueryActionFilter"/>.
/// </summary>
internal static partial class QueryActionFilterLogMessages
{
    [LoggerMessage(0, LogLevel.Trace, "Controller {Controller} with action {Action} returns a client observable")]
    internal static partial void ClientObservableReturnValue(this ILogger<QueryActionFilter> logger, string controller, string action);

    [LoggerMessage(1, LogLevel.Trace, "Request is WebSocket")]
    internal static partial void RequestIsWebSocket(this ILogger<QueryActionFilter> logger);

    [LoggerMessage(2, LogLevel.Trace, "Request is regular HTTP")]
    internal static partial void RequestIsHttp(this ILogger<QueryActionFilter> logger);

    [LoggerMessage(3, LogLevel.Trace, "Controller {Controller} with action {Action} returns a regular object")]
    internal static partial void NonClientObservableReturnValue(this ILogger<QueryActionFilter> logger, string controller, string action);

    [LoggerMessage(4, LogLevel.Trace, "WebSocket headers are as follows: Protocol='{Protocol}' Extensions='{Extensions}' Version='{Version}' Key='{Key}'")]
    internal static partial void DumpWebSocketHeaders(this ILogger<QueryActionFilter> logger, string protocol, string extensions, string version, string key);

    [LoggerMessage(5, LogLevel.Trace, "Controller {Controller} with action {Action} returns a client enumerable")]
    internal static partial void AsyncEnumerableReturnValue(this ILogger<QueryActionFilter> logger, string controller, string action);
}
