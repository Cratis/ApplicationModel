// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Reactive.Subjects;
using Cratis.Applications.Validation;
using Cratis.Reflection;
using Cratis.Strings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents a <see cref="IAsyncActionFilter"/> for providing a proper <see cref="QueryResult{T}"/> for post actions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QueryActionFilter"/> class.
/// </remarks>
/// <param name="options"><see cref="JsonOptions"/>.</param>
/// <param name="queryContextManager"><see cref="IQueryContextManager"/>.</param>
/// <param name="queryProviders"><see cref="IQueryProviders"/>.</param>
/// <param name="logger"><see cref="ILogger"/> for logging.</param>
public class QueryActionFilter(
    IOptions<JsonOptions> options,
    IQueryContextManager queryContextManager,
    IQueryProviders queryProviders,
    ILogger<QueryActionFilter> logger) : IAsyncActionFilter
{
    /// <summary>
    /// Gets the key for the sort by query string.
    /// </summary>
    public const string SortByQueryStringKey = "sortby";

    /// <summary>
    /// Gets the key for the sort direction query string.
    /// </summary>
    public const string SortDirectionQueryStringKey = "sortDirection";

    /// <summary>
    /// Gets the key for the page query string.
    /// </summary>
    public const string PageQueryStringKey = "page";

    /// <summary>
    /// Gets the key for the page size query string.
    /// </summary>
    public const string PageSizeQueryStringKey = "pageSize";

    readonly JsonOptions _options = options.Value;

    /// <inheritdoc/>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Method == HttpMethod.Get.Method &&
            context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            EstablishQueryContext(context);

            var callResult = await CallNextAndHandleValidationAndExceptions(context, next);
            if (context.IsAspNetResult()) return;

            if (callResult.Result?.Result is ObjectResult objectResult && IsStreamingResult(objectResult))
            {
                if (IsSubjectResult(objectResult))
                {
                    logger.ClientObservableReturnValue(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
                    var clientObservable = CreateClientObservableFrom(context.HttpContext.RequestServices, objectResult);
                    HandleWebSocketHeadersForMultipleProxies(context.HttpContext);

                    if (context.HttpContext.WebSockets.IsWebSocketRequest)
                    {
                        logger.RequestIsWebSocket();
                        await clientObservable.HandleConnection(context);
                        callResult.Result.Result = null;
                    }
                    else
                    {
                        logger.RequestIsHttp();
                        callResult.Result.Result = new ObjectResult(clientObservable);
                    }
                }
                else if (IsAsyncEnumerableResult(objectResult))
                {
                    logger.AsyncEnumerableReturnValue(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
                    var clientEnumerableObservable = CreateClientEnumerableObservableFrom(context.HttpContext.RequestServices, objectResult);

                    HandleWebSocketHeadersForMultipleProxies(context.HttpContext);
                    if (context.HttpContext.WebSockets.IsWebSocketRequest)
                    {
                        logger.RequestIsWebSocket();
                        await clientEnumerableObservable.HandleConnection(context);
                    }
                    else
                    {
                        logger.RequestIsHttp();
                        callResult.Result.Result = new ObjectResult(objectResult.Value);
                    }
                }
            }
            else
            {
                logger.NonClientObservableReturnValue(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
                var response = callResult.Response is not null ? queryProviders.Execute(callResult.Response!) : new QueryProviderResult(0, default!);
                var queryContext = queryContextManager.Current;
                var queryResult = new QueryResult<object>
                {
                    Paging = queryContext.Paging == Paging.NotPaged ? PagingInfo.NotPaged : new PagingInfo(
                        queryContext.Paging.Page,
                        queryContext.Paging.Size,
                        response.TotalItems),
                    CorrelationId = context.HttpContext.GetCorrelationId(),
                    ValidationResults = context.ModelState.SelectMany(_ => _.Value!.Errors.Select(p => p.ToValidationResult(_.Key.ToCamelCase()))),
                    ExceptionMessages = callResult.ExceptionMessages,
                    ExceptionStackTrace = callResult.ExceptionStackTrace ?? string.Empty,
                    Data = response.Data
                };

                if (response.Data is null && queryResult.IsSuccess)
                {
                    queryResult.ExceptionMessages = ["Null data returned"];
                }

                if (!queryResult.IsAuthorized)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;         // Forbidden: https://www.rfc-editor.org/rfc/rfc9110.html#name-403-forbidden
                }
                else if (!queryResult.IsValid)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;           // Bad request: https://www.rfc-editor.org/rfc/rfc9110.html#name-400-bad-request
                }
                else if (queryResult.HasExceptions)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;  // Internal Server error: https://www.rfc-editor.org/rfc/rfc9110.html#name-500-internal-server-error
                }

                var actualResult = new ObjectResult(queryResult);

                if (callResult.Result is not null)
                {
                    callResult.Result.Result = actualResult;
                }
                else
                {
                    context.Result = actualResult;
                }
            }
        }
        else
        {
            await next();
        }
    }

    void EstablishQueryContext(ActionExecutingContext context)
    {
        var sorting = Sorting.None;
        var paging = Paging.NotPaged;

        if (context.HttpContext.Request.Query.ContainsKey(SortByQueryStringKey) &&
            context.HttpContext.Request.Query.ContainsKey(SortDirectionQueryStringKey))
        {
            sorting = new Sorting(
                context.HttpContext.Request.Query[SortByQueryStringKey].ToString()!.ToPascalCase(),
                context.HttpContext.Request.Query[SortDirectionQueryStringKey].ToString()! == "desc" ? SortDirection.Descending : SortDirection.Ascending);
        }

        if (context.HttpContext.Request.Query.ContainsKey(PageQueryStringKey) &&
            context.HttpContext.Request.Query.ContainsKey(PageSizeQueryStringKey))
        {
            var page = int.Parse(context.HttpContext.Request.Query[PageQueryStringKey].ToString()!);
            var pageSize = int.Parse(context.HttpContext.Request.Query[PageSizeQueryStringKey].ToString()!);
            paging = new(page, pageSize, true);
        }

        queryContextManager.Set(new(context.HttpContext.GetCorrelationId(), paging, sorting));
    }

    IClientEnumerableObservable CreateClientEnumerableObservableFrom(IServiceProvider serviceProvider, ObjectResult objectResult)
    {
        var type = objectResult.Value!.GetType();
        var clientEnumerableObservableType = typeof(ClientEnumerableObservable<>).MakeGenericType(type.GetGenericArguments()[0]);
        return (ActivatorUtilities.CreateInstance(serviceProvider, clientEnumerableObservableType, objectResult.Value, _options) as IClientEnumerableObservable)!;
    }

    IClientObservable CreateClientObservableFrom(IServiceProvider serviceProvider, ObjectResult objectResult)
    {
        var type = objectResult.Value!.GetType();
        var subjectType = type.GetInterfaces().First(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(ISubject<>));
        var clientObservableType = typeof(ClientObservable<>).MakeGenericType(subjectType.GetGenericArguments()[0]);
        return (ActivatorUtilities.CreateInstance(serviceProvider, clientObservableType, queryContextManager.Current, objectResult.Value, _options) as IClientObservable)!;
    }

    bool IsStreamingResult(ObjectResult objectResult) => IsAsyncEnumerableResult(objectResult) || IsSubjectResult(objectResult);

    bool IsAsyncEnumerableResult(ObjectResult objectResult) =>
        objectResult.Value?.GetType().ImplementsOpenGeneric(typeof(IAsyncEnumerable<>)) ?? false;

    bool IsSubjectResult(ObjectResult objectResult) =>
        objectResult.Value?.GetType().ImplementsOpenGeneric(typeof(ISubject<>)) ?? false;

    /// <summary>
    /// Handles the Web Socket headers for connections that are going through multiple proxies.
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/> to handle for.</param>
    /// <remarks>
    /// In the ASP.NET Core 6 code there is a middleware called WebSocketMiddleware. The WebSocketManager
    /// that we ask for .IsWebSocketRequest forwards this call to it.
    /// This property calls internally a method called CheckSupportedWebSocketRequest which will check
    /// the following Http Headers for valid values:
    /// Upgrade with value Upgrade
    /// Connection with value websocket
    /// <p/>
    /// If they are correct, it will consider it an upgrade of the protocol and will then validate and use
    /// the values from the Web Socket specific headers:
    /// Sec-WebSocket-Protocol
    /// Sec-WebSocket-Extensions
    /// Sec-WebSocket-Version
    /// Sec-WebSocket-Key
    /// <p/>
    /// When running in an environment with multiple reverse proxies you can end up with the proxy adding
    /// to the values if the values are already there, forming a collection of values as comma separated
    /// values in the HTTP header.
    /// The validation code in ASP.NET validates that the version is supported and that the key is valid.
    /// Throughout the validation code in ASP.NET it recognizes the fact that it could hold multiple values
    /// and loops through the values, except for the key - which it just does .ToString() on, which then
    /// gives you the comma separated string.
    /// The key is expected to be a base64 encoded byte array of 16 bytes, and obviously this would not
    /// then be valid and we're not allowed to upgrade the connection.
    /// The purpose of the key coming from the client is to use it and combine with a server key and send
    /// back on the response to form a valid connection.
    /// This code basically recognizes this problem and assumes that the last key is the one from the client
    /// and strips away any other keys and uses it instead.
    /// </remarks>
    void HandleWebSocketHeadersForMultipleProxies(HttpContext httpContext)
    {
        logger.DumpWebSocketHeaders(
            httpContext.Request.Headers.SecWebSocketProtocol.ToString(),
            httpContext.Request.Headers.SecWebSocketExtensions.ToString(),
            httpContext.Request.Headers.SecWebSocketVersion.ToString(),
            httpContext.Request.Headers.SecWebSocketKey.ToString());
        var keys = httpContext.Request.Headers.SecWebSocketKey.ToString().Split(',').Select(_ => _.Trim()).ToArray();
        if (keys.Length > 1)
        {
            httpContext.Request.Headers.SecWebSocketKey = keys[^1];
        }
    }

    async Task<(ActionExecutedContext? Result, IEnumerable<string> ExceptionMessages, string? ExceptionStackTrace, object? Response)> CallNextAndHandleValidationAndExceptions(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var exceptionMessages = new List<string>();
        var exceptionStackTrace = string.Empty;
        object? response = null;
        ActionExecutedContext? result = null;

        if (context.ModelState.IsValid || context.ShouldIgnoreValidation())
        {
            result = await next();

            if (context.IsAspNetResult())
            {
                return (null, exceptionMessages, exceptionStackTrace, response);
            }

            if (result.Exception is not null)
            {
                var exception = result.Exception;
                exceptionStackTrace = exception.StackTrace;

                do
                {
                    exceptionMessages.Add(exception.Message);
                    exception = exception.InnerException;
                }
                while (exception is not null);

                result.Exception = null!;
            }

            if (result.Result is ObjectResult objectResult)
            {
                response = objectResult.Value;
            }
        }

        return (result, exceptionMessages, exceptionStackTrace, response);
    }
}
