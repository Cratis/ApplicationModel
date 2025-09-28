// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Reactive.Subjects;
using Cratis.Applications.Validation;
using Cratis.Reflection;
using Cratis.Strings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Queries;

/// <summary>
/// Provides common query processing functionality that can be used by both controller-based queries and pipeline-based queries.
/// </summary>
public static class QueryProcessingHelper
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

    /// <summary>
    /// Establishes the query context from the HTTP request.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="queryName">The name of the query.</param>
    /// <param name="queryContextManager">The query context manager.</param>
    /// <returns>The established query context.</returns>
    public static QueryContext EstablishQueryContext(HttpContext httpContext, QueryName queryName, IQueryContextManager queryContextManager)
    {
        var sorting = ExtractSorting(httpContext);
        var paging = ExtractPaging(httpContext);
        var correlationId = httpContext.GetCorrelationId();

        var queryContext = new QueryContext(queryName, correlationId, paging, sorting);
        queryContextManager.Set(queryContext);
        return queryContext;
    }

    /// <summary>
    /// Extracts paging information from the HTTP request query string.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The paging information.</returns>
    public static Paging ExtractPaging(HttpContext httpContext)
    {
        if (httpContext.Request.Query.ContainsKey(PageQueryStringKey) &&
            httpContext.Request.Query.ContainsKey(PageSizeQueryStringKey) &&
            int.TryParse(httpContext.Request.Query[PageQueryStringKey].ToString(), out var page) &&
            int.TryParse(httpContext.Request.Query[PageSizeQueryStringKey].ToString(), out var pageSize))
        {
            return new Paging(page, pageSize, true);
        }

        return Paging.NotPaged;
    }

    /// <summary>
    /// Extracts sorting information from the HTTP request query string.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The sorting information.</returns>
    public static Sorting ExtractSorting(HttpContext httpContext)
    {
        if (httpContext.Request.Query.ContainsKey(SortByQueryStringKey) &&
            httpContext.Request.Query.ContainsKey(SortDirectionQueryStringKey))
        {
            var sortBy = httpContext.Request.Query[SortByQueryStringKey].ToString()?.ToPascalCase();
            var sortDirection = httpContext.Request.Query[SortDirectionQueryStringKey].ToString();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortDirection))
            {
                var direction = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? SortDirection.Descending
                    : SortDirection.Ascending;
                return new Sorting(sortBy, direction);
            }
        }
        return Sorting.None;
    }

    /// <summary>
    /// Sets the HTTP response status code based on the query result.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="queryResult">The query result.</param>
    public static void SetResponseStatusCode(HttpResponse response, QueryResult queryResult)
    {
        if (!queryResult.IsAuthorized)
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        else if (!queryResult.IsValid)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (queryResult.HasExceptions)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        // else 200 OK (default)
    }

    /// <summary>
    /// Creates a query result from the response data and query context.
    /// </summary>
    /// <param name="response">The response data.</param>
    /// <param name="queryName">The name of the query.</param>
    /// <param name="queryContext">The query context.</param>
    /// <param name="exceptionMessages">Any exception messages.</param>
    /// <param name="exceptionStackTrace">Any exception stack trace.</param>
    /// <param name="validationResults">Any validation results.</param>
    /// <param name="queryProviders">The query providers for rendering.</param>
    /// <returns>The query result.</returns>
    public static QueryResult CreateQueryResult(
        object? response,
        QueryName queryName,
        QueryContext queryContext,
        IEnumerable<string> exceptionMessages,
        string exceptionStackTrace,
        IEnumerable<ValidationResult> validationResults,
        IQueryRenderers queryProviders)
    {
        var rendererResult = response is not null ? queryProviders.Render(queryName, response) : new QueryRendererResult(0, default!);

        var queryResult = new QueryResult
        {
            Paging = queryContext.Paging == Paging.NotPaged ? PagingInfo.NotPaged : new PagingInfo(
                queryContext.Paging.Page,
                queryContext.Paging.Size,
                rendererResult.TotalItems),
            CorrelationId = queryContext.CorrelationId,
            ValidationResults = validationResults,
            ExceptionMessages = exceptionMessages,
            ExceptionStackTrace = exceptionStackTrace,
            Data = rendererResult.Data
        };

        if (rendererResult.Data is null && queryResult.IsSuccess)
        {
            queryResult.ExceptionMessages = ["Null data returned"];
        }

        return queryResult;
    }

    /// <summary>
    /// Handles WebSocket headers for connections that are going through multiple proxies.
    /// </summary>
    /// <param name="httpContext">The HTTP context to handle for.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public static void HandleWebSocketHeadersForMultipleProxies(HttpContext httpContext, ILogger? logger = null)
    {
#pragma warning disable CA1848 // Use LoggerMessage delegates for improved performance - acceptable for this diagnostic scenario
        logger?.LogDebug(
            "WebSocket Headers: Protocol={Protocol}, Extensions={Extensions}, Version={Version}, Key={Key}",
            httpContext.Request.Headers.SecWebSocketProtocol.ToString(),
            httpContext.Request.Headers.SecWebSocketExtensions.ToString(),
            httpContext.Request.Headers.SecWebSocketVersion.ToString(),
            httpContext.Request.Headers.SecWebSocketKey.ToString());
#pragma warning restore CA1848

        var keys = httpContext.Request.Headers.SecWebSocketKey.ToString().Split(',').Select(_ => _.Trim()).ToArray();
        if (keys.Length > 1)
        {
            httpContext.Request.Headers.SecWebSocketKey = keys[^1];
        }
    }

    /// <summary>
    /// Determines if the result is a streaming result (Subject or AsyncEnumerable).
    /// </summary>
    /// <param name="objectResult">The object result to check.</param>
    /// <returns>True if it's a streaming result, false otherwise.</returns>
    public static bool IsStreamingResult(ObjectResult objectResult) =>
        IsAsyncEnumerableResult(objectResult) || IsSubjectResult(objectResult);

    /// <summary>
    /// Determines if the result is an AsyncEnumerable result.
    /// </summary>
    /// <param name="objectResult">The object result to check.</param>
    /// <returns>True if it's an AsyncEnumerable result, false otherwise.</returns>
    public static bool IsAsyncEnumerableResult(ObjectResult objectResult) =>
        objectResult.Value?.GetType().ImplementsOpenGeneric(typeof(IAsyncEnumerable<>)) ?? false;

    /// <summary>
    /// Determines if the result is a Subject result.
    /// </summary>
    /// <param name="objectResult">The object result to check.</param>
    /// <returns>True if it's a Subject result, false otherwise.</returns>
    public static bool IsSubjectResult(ObjectResult objectResult) =>
        objectResult.Value?.GetType().ImplementsOpenGeneric(typeof(ISubject<>)) ?? false;

    /// <summary>
    /// Creates a client observable from an object result.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="objectResult">The object result.</param>
    /// <param name="queryContextManager">The query context manager.</param>
    /// <param name="options">The JSON options.</param>
    /// <returns>The client observable.</returns>
    public static IClientObservable CreateClientObservableFrom(
        IServiceProvider serviceProvider,
        ObjectResult objectResult,
        IQueryContextManager queryContextManager,
        JsonOptions options)
    {
        var type = objectResult.Value!.GetType();
        var subjectType = type.GetInterfaces().First(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(ISubject<>));
        var clientObservableType = typeof(ClientObservable<>).MakeGenericType(subjectType.GetGenericArguments()[0]);
        return (ActivatorUtilities.CreateInstance(serviceProvider, clientObservableType, queryContextManager.Current, objectResult.Value, options) as IClientObservable)!;
    }

    /// <summary>
    /// Creates a client enumerable observable from an object result.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="objectResult">The object result.</param>
    /// <param name="options">The JSON options.</param>
    /// <returns>The client enumerable observable.</returns>
    public static IClientEnumerableObservable CreateClientEnumerableObservableFrom(
        IServiceProvider serviceProvider,
        ObjectResult objectResult,
        JsonOptions options)
    {
        var type = objectResult.Value!.GetType();
        var clientEnumerableObservableType = typeof(ClientEnumerableObservable<>).MakeGenericType(type.GetGenericArguments()[0]);
        return (ActivatorUtilities.CreateInstance(serviceProvider, clientEnumerableObservableType, objectResult.Value, options) as IClientEnumerableObservable)!;
    }
}