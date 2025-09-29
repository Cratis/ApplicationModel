// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using Cratis.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Queries;

/// <summary>
/// Provides extension methods for working with observable queries.
/// </summary>
public static class ObservableQueryExtensions
{
    /// <summary>
    /// Handles WebSocket headers for connections that are going through multiple proxies.
    /// </summary>
    /// <param name="httpContext">The HTTP context to handle for.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public static void HandleWebSocketHeadersForMultipleProxies(this HttpContext httpContext, ILogger? logger = null)
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
    public static bool IsStreamingResult(this ObjectResult objectResult) =>
        IsAsyncEnumerableResult(objectResult) || IsSubjectResult(objectResult);

    /// <summary>
    /// Determines if the result is an AsyncEnumerable result.
    /// </summary>
    /// <param name="objectResult">The object result to check.</param>
    /// <returns>True if it's an AsyncEnumerable result, false otherwise.</returns>
    public static bool IsAsyncEnumerableResult(this ObjectResult objectResult) =>
        objectResult.Value?.GetType().ImplementsOpenGeneric(typeof(IAsyncEnumerable<>)) ?? false;

    /// <summary>
    /// Determines if the result is a Subject result.
    /// </summary>
    /// <param name="objectResult">The object result to check.</param>
    /// <returns>True if it's a Subject result, false otherwise.</returns>
    public static bool IsSubjectResult(this ObjectResult objectResult) =>
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