// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Driver;

namespace Cratis.Applications.MongoDB.Resilience;

/// <summary>
/// Represents an empty <see cref="IChangeStreamCursor{TDocument}"/> that returns no documents.
/// </summary>
/// <typeparam name="TDocument">Type of document.</typeparam>
/// <remarks>
/// Used to mimic regular MongoDB behavior when a collection doesn't exist in Azure Cosmos DB.
/// Unlike EmptyAsyncCursor which returns false immediately, this cursor blocks indefinitely
/// (respecting cancellation) to keep change stream observations alive until the collection exists.
/// </remarks>
public class EmptyChangeStreamCursor<TDocument> : IChangeStreamCursor<TDocument>
{
    readonly TaskCompletionSource<bool> _blockingTask = new();

    /// <inheritdoc/>
    public IEnumerable<TDocument> Current => [];

    /// <inheritdoc/>
    public BsonDocument? GetResumeToken() => null;

    /// <inheritdoc/>
    public bool MoveNext(CancellationToken cancellationToken = default)
    {
        try
        {
            _blockingTask.Task.Wait(cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _blockingTask.Task.WaitAsync(cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _blockingTask.TrySetCanceled();
    }
}
