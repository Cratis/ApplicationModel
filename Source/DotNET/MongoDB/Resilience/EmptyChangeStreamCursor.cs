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
/// </remarks>
public class EmptyChangeStreamCursor<TDocument> : IChangeStreamCursor<TDocument>
{
    /// <inheritdoc/>
    public IEnumerable<TDocument> Current => [];

    /// <inheritdoc/>
    public BsonDocument? GetResumeToken() => null;

    /// <inheritdoc/>
    public bool MoveNext(CancellationToken cancellationToken = default) => false;

    /// <inheritdoc/>
    public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default) => Task.FromResult(false);

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
