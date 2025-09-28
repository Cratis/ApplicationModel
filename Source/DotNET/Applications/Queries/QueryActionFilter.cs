// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Validation;
using Cratis.Strings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents a <see cref="IAsyncActionFilter"/> for providing a proper <see cref="QueryResult"/> for post actions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QueryActionFilter"/> class.
/// </remarks>
/// <param name="options"><see cref="JsonOptions"/>.</param>
/// <param name="queryContextManager"><see cref="IQueryContextManager"/>.</param>
/// <param name="queryProviders"><see cref="IQueryRenderers"/>.</param>
/// <param name="logger"><see cref="ILogger"/> for logging.</param>
public class QueryActionFilter(
    IOptions<JsonOptions> options,
    IQueryContextManager queryContextManager,
    IQueryRenderers queryProviders,
    ILogger<QueryActionFilter> logger) : IAsyncActionFilter
{
    readonly JsonOptions _options = options.Value;

    /// <inheritdoc/>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Method == HttpMethod.Get.Method &&
            context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var queryContext = QueryProcessingHelper.EstablishQueryContext(context.HttpContext, context.ActionDescriptor.DisplayName ?? "[NotSet]", queryContextManager);

            var callResult = await CallNextAndHandleValidationAndExceptions(context, next);
            if (context.IsAspNetResult()) return;

            if (callResult.Result?.Result is ObjectResult objectResult && QueryProcessingHelper.IsStreamingResult(objectResult))
            {
                if (QueryProcessingHelper.IsSubjectResult(objectResult))
                {
                    logger.ClientObservableReturnValue(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
                    var clientObservable = QueryProcessingHelper.CreateClientObservableFrom(context.HttpContext.RequestServices, objectResult, queryContextManager, _options);
                    QueryProcessingHelper.HandleWebSocketHeadersForMultipleProxies(context.HttpContext, logger);

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
                else if (QueryProcessingHelper.IsAsyncEnumerableResult(objectResult))
                {
                    logger.AsyncEnumerableReturnValue(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName);
                    var clientEnumerableObservable = QueryProcessingHelper.CreateClientEnumerableObservableFrom(context.HttpContext.RequestServices, objectResult, _options);

                    QueryProcessingHelper.HandleWebSocketHeadersForMultipleProxies(context.HttpContext, logger);
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
                var validationResults = context.ModelState.SelectMany(_ => _.Value!.Errors.Select(p => p.ToValidationResult(_.Key.ToCamelCase())));
                var queryResult = QueryProcessingHelper.CreateQueryResult(
                    callResult.Response,
                    queryContext.Name,
                    queryContext,
                    callResult.ExceptionMessages,
                    callResult.ExceptionStackTrace ?? string.Empty,
                    validationResults,
                    queryProviders);

                QueryProcessingHelper.SetResponseStatusCode(context.HttpContext.Response, queryResult);

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
