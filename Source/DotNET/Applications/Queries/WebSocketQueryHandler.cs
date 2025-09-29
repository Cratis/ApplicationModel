// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents an implementation of <see cref="IWebSocketQueryHandler"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WebSocketQueryHandler"/> class.
/// </remarks>
/// <param name="queryContextManager"><see cref="IQueryContextManager"/>.</param>
/// <param name="options"><see cref="JsonOptions"/>.</param>
/// <param name="logger"><see cref="ILogger"/> for logging.</param>
[Singleton]
public class WebSocketQueryHandler(
    IQueryContextManager queryContextManager,
    IOptions<JsonOptions> options,
    ILogger<WebSocketQueryHandler> logger) : IWebSocketQueryHandler
{
    readonly JsonOptions _options = options.Value;

    /// <inheritdoc/>
    public bool ShouldHandleAsWebSocket(ActionExecutingContext context) =>
        context.HttpContext.WebSockets.IsWebSocketRequest;

    /// <inheritdoc/>
    public async Task HandleStreamingResult(
        ActionExecutingContext context,
        ActionExecutedContext? actionExecutedContext,
        ObjectResult objectResult)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
        {
            return;
        }

        context.HttpContext.HandleWebSocketHeadersForMultipleProxies(logger);

        if (objectResult.IsSubjectResult())
        {
            await HandleSubjectResult(context, actionExecutedContext, objectResult, controllerActionDescriptor);
        }
        else if (objectResult.IsAsyncEnumerableResult())
        {
            await HandleAsyncEnumerableResult(context, actionExecutedContext, objectResult, controllerActionDescriptor);
        }
    }

    async Task HandleSubjectResult(
        ActionExecutingContext context,
        ActionExecutedContext? callResult,
        ObjectResult objectResult,
        ControllerActionDescriptor controllerActionDescriptor)
    {
        logger.ClientObservableReturnValue(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
        var clientObservable = ObservableQueryExtensions.CreateClientObservableFrom(
            context.HttpContext.RequestServices,
            objectResult,
            queryContextManager,
            _options);

        if (ShouldHandleAsWebSocket(context))
        {
            logger.RequestIsWebSocket();
            await clientObservable.HandleConnection(context);
            if (callResult?.Result is ObjectResult objResult)
            {
                objResult.Value = null;
            }
        }
        else
        {
            logger.RequestIsHttp();
            if (callResult?.Result is ObjectResult objResult)
            {
                objResult.Value = clientObservable;
            }
            else if (callResult is not null)
            {
                callResult.Result = new ObjectResult(clientObservable);
            }
        }
    }

    async Task HandleAsyncEnumerableResult(
        ActionExecutingContext context,
        ActionExecutedContext? callResult,
        ObjectResult objectResult,
        ControllerActionDescriptor controllerActionDescriptor)
    {
        logger.AsyncEnumerableReturnValue(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
        var clientEnumerableObservable = ObservableQueryExtensions.CreateClientEnumerableObservableFrom(
            context.HttpContext.RequestServices,
            objectResult,
            _options);

        if (ShouldHandleAsWebSocket(context))
        {
            logger.RequestIsWebSocket();
            await clientEnumerableObservable.HandleConnection(context);
        }
        else
        {
            logger.RequestIsHttp();
            if (callResult?.Result is ObjectResult objResult)
            {
                objResult.Value = objectResult.Value;
            }
            else if (callResult is not null)
            {
                callResult.Result = new ObjectResult(objectResult.Value);
            }
        }
    }
}