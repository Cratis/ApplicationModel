// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Subjects;

namespace Read.Products;

/// <summary>
/// Represents an implementation of <see cref="ICartQueries"/>.
/// </summary>
/// <param name="collection">The <see cref="IMongoCollection{TDocument}"/> for <see cref="Product"/>.</param>
public class CatalogQueries(IMongoCollection<Product> collection) : ICatalogQueries
{
    /// <inheritdoc/>
    public IQueryable<Product> All() => collection.AsQueryable();

    /// <inheritdoc/>
    public ISubject<IEnumerable<Product>> ObserveAll() => collection.Observe(_ => _.IsRegistered);
}