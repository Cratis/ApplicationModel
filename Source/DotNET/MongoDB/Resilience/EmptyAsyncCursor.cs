// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Cratis.Arc.MongoDB.Resilience;

/// <summary>
/// Represents an empty <see cref="IAsyncCursor{TDocument}"/> that returns no documents.
/// </summary>
/// <typeparam name="TDocument">Type of document.</typeparam>
/// <remarks>
/// Used to mimic regular MongoDB behavior when a collection doesn't exist in Azure Cosmos DB.
/// </remarks>
public class EmptyAsyncCursor<TDocument> : IAsyncCursor<TDocument>
{
    /// <inheritdoc/>
    public IEnumerable<TDocument> Current => [];

    /// <inheritdoc/>
    public bool MoveNext(CancellationToken cancellationToken = default) => false;

    /// <inheritdoc/>
    public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default) => Task.FromResult(false);

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
