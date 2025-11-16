// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Net.ServerSentEvents;
using System.Text.Json;
using Cratis.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents an implementation of <see cref="IServerSentEventsConnectionHandler"/>.
/// </summary>
/// <param name="handlerLogger">The <see cref="ILogger"/>.</param>
[Singleton]
public class ServerSentEventsConnectionHandler(ILogger<ServerSentEventsConnectionHandler> handlerLogger) : IServerSentEventsConnectionHandler
{
    /// <inheritdoc/>
    public async Task StreamQueryResults<T>(
        HttpContext httpContext,
        IAsyncEnumerable<QueryResult> items,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken,
        ILogger? logger = null)
    {
        httpContext.Response.ContentType = "text/event-stream";
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";

        try
        {
            var sseItems = ConvertToSseItems(items, cancellationToken);

            await SseFormatter.WriteAsync(
                sseItems,
                httpContext.Response.Body,
                (item, writer) => SerializeQueryResult(item, writer, jsonSerializerOptions),
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            handlerLogger.SseConnectionCancelled();
        }
        catch (Exception ex)
        {
            handlerLogger.SseStreamingError(ex);
        }
    }

    static async IAsyncEnumerable<SseItem<QueryResult>> ConvertToSseItems(
        IAsyncEnumerable<QueryResult> items,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in items.WithCancellation(cancellationToken))
        {
            yield return new SseItem<QueryResult>(item, "message");
        }
    }

    static void SerializeQueryResult(
        SseItem<QueryResult> item,
        IBufferWriter<byte> writer,
        JsonSerializerOptions jsonSerializerOptions)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(item.Data, jsonSerializerOptions);
        writer.Write(json);
    }
}
