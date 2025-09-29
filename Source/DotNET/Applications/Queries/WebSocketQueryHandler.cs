// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using Cratis.DependencyInjection;
using Cratis.Reflection;
using Microsoft.AspNetCore.Http;
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
    public bool ShouldHandleAsWebSocket(HttpContext httpContext) =>
        httpContext.WebSockets.IsWebSocketRequest;

    /// <inheritdoc/>
    public bool IsStreamingResult(object? data) =>
        data?.GetType().ImplementsOpenGeneric(typeof(ISubject<>)) is true ||
        data?.GetType().ImplementsOpenGeneric(typeof(IAsyncEnumerable<>)) is true;

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

    /// <inheritdoc/>
    public async Task HandleStreamingResult(
        HttpContext httpContext,
        QueryName queryName,
        object streamingData,
        QueryContext queryContext)
    {
        httpContext.HandleWebSocketHeadersForMultipleProxies(logger);

        if (IsSubjectResult(streamingData))
        {
            await HandleSubjectResultForEndpoint(httpContext, queryName, streamingData);
        }
        else if (IsAsyncEnumerableResult(streamingData))
        {
            await HandleAsyncEnumerableResultForEndpoint(httpContext, queryName, streamingData);
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
            await clientEnumerableObservable.HandleConnection(context.HttpContext);
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

    bool IsSubjectResult(object data) =>
        data.GetType().ImplementsOpenGeneric(typeof(ISubject<>));

    bool IsAsyncEnumerableResult(object data) =>
        data.GetType().ImplementsOpenGeneric(typeof(IAsyncEnumerable<>));

    async Task HandleSubjectResultForEndpoint(
        HttpContext httpContext,
        QueryName queryName,
        object streamingData)
    {
        logger.EndpointObservableReturnValue(queryName);
        var objectResult = new ObjectResult(streamingData);
        var clientObservable = ObservableQueryExtensions.CreateClientObservableFrom(
            httpContext.RequestServices,
            objectResult,
            queryContextManager,
            _options);

        if (ShouldHandleAsWebSocket(httpContext))
        {
            logger.RequestIsWebSocket();
            await HandleWebSocketConnection(httpContext, clientObservable);
        }
        else
        {
            logger.RequestIsHttp();
            await httpContext.Response.WriteAsJsonAsync(clientObservable, _options.JsonSerializerOptions, cancellationToken: httpContext.RequestAborted);
        }
    }

    async Task HandleAsyncEnumerableResultForEndpoint(
        HttpContext httpContext,
        QueryName queryName,
        object streamingData)
    {
        logger.EndpointEnumerableReturnValue(queryName);
        var objectResult = new ObjectResult(streamingData);
        var clientEnumerableObservable = ObservableQueryExtensions.CreateClientEnumerableObservableFrom(
            httpContext.RequestServices,
            objectResult,
            _options);

        if (ShouldHandleAsWebSocket(httpContext))
        {
            logger.RequestIsWebSocket();
            await HandleWebSocketConnection(httpContext, clientEnumerableObservable);
        }
        else
        {
            logger.RequestIsHttp();
            await httpContext.Response.WriteAsJsonAsync(streamingData, _options.JsonSerializerOptions, cancellationToken: httpContext.RequestAborted);
        }
    }

    async Task HandleWebSocketConnection(HttpContext httpContext, IClientObservable clientObservable)
    {
        await clientObservable.HandleConnection(httpContext);
    }

    async Task HandleWebSocketConnection(HttpContext httpContext, IClientEnumerableObservable clientEnumerableObservable)
    {
        await clientEnumerableObservable.HandleConnection(httpContext);
    }
}