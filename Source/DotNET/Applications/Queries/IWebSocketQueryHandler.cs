// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a handler for WebSocket-based query operations.
/// </summary>
public interface IWebSocketQueryHandler
{
    /// <summary>
    /// Handles a streaming query result for WebSocket connections.
    /// </summary>
    /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>
    /// <param name="actionExecutedContext">The <see cref="ActionExecutedContext"/> from the action execution.</param>
    /// <param name="objectResult">The <see cref="ObjectResult"/> containing the streaming data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleStreamingResult(
        ActionExecutingContext context,
        ActionExecutedContext? actionExecutedContext,
        ObjectResult objectResult);

    /// <summary>
    /// Determines if the current request should be handled as a WebSocket connection.
    /// </summary>
    /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>
    /// <returns>True if the request should be handled as WebSocket, false otherwise.</returns>
    bool ShouldHandleAsWebSocket(ActionExecutingContext context);
}