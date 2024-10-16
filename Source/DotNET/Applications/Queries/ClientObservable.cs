// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents an implementation of <see cref="IClientObservable"/>.
/// </summary>
/// <typeparam name="T">Type of data being observed.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ClientObservable{T}"/> class.
/// </remarks>
/// <param name="queryContext">The <see cref="QueryContext"/> the observable is for.</param>
/// <param name="subject">The <see cref="ISubject{T}"/> the observable wraps.</param>
/// <param name="jsonOptions">The <see cref="JsonOptions"/>.</param>
public class ClientObservable<T>(
    QueryContext queryContext,
    ISubject<T> subject,
    JsonOptions jsonOptions) : IClientObservable, IAsyncEnumerable<T>
{
    const int BufferSIze = 1024 * 4;

    /// <summary>
    /// Notifies all subscribed and future observers about the arrival of the specified element in the sequence.
    /// </summary>
    /// <param name="next">The value to send to all observers.</param>
    public void OnNext(T next) => subject.OnNext(next);

    /// <inheritdoc/>
    public async Task HandleConnection(ActionExecutingContext context)
    {
        using var webSocket = await context.HttpContext.WebSockets.AcceptWebSocketAsync();
        IDisposable? subscription = default;
        var queryResult = new QueryResult<object>();
        using var cts = new CancellationTokenSource();

#pragma warning disable MA0147 // Avoid async void method for delegate
        subscription = subject.Subscribe(async _ =>
        {
            queryResult.Paging = new(
                queryContext.Paging.Page,
                queryContext.Paging.Size,
                queryContext.TotalItems);

            queryResult.Data = _!;

            try
            {
                var message = JsonSerializer.SerializeToUtf8Bytes(queryResult, jsonOptions.JsonSerializerOptions);
                await webSocket.SendAsync(message, WebSocketMessageType.Text, true, cts.Token);
                message = null!;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending message to client: {ex.Message}\n{ex.StackTrace}");
                subscription?.Dispose();
                subject.OnCompleted();
            }
        });
#pragma warning restore MA0147 // Avoid async void method for delegate

        var buffer = new byte[BufferSIze];
        try
        {
            var received = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

            while (!received.CloseStatus.HasValue)
            {
                received = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            }

            await webSocket.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, cts.Token);
        }
        finally
        {
            Console.WriteLine("Client disconnected");
            cts.Cancel();
            subject.OnCompleted();
            subscription?.Dispose();
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new ObservableAsyncEnumerator<T>(subject, cancellationToken);

    /// <inheritdoc/>
    public object GetAsynchronousEnumerator(CancellationToken cancellationToken = default) => GetAsyncEnumerator(cancellationToken);
}
