// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications;
using Domain.Products;
using MongoDB.Driver;
using Read.Products;

namespace API.Products;

/// <summary>
/// Represents the catalog API.
/// </summary>
/// <param name="grainFactory"><see cref="IGrainFactory"/> for working with grains.</param>
/// <param name="catalogQueries"><see cref="ICatalogQueries"/> for working with the catalog.</param>
/// <param name="collection"><see cref="IMongoCollection{TDocument}"/> for working with products.</param>
[Route("/api/products/catalog")]
public class Catalog(IGrainFactory grainFactory, ICatalogQueries catalogQueries, IMongoCollection<Product> collection) : ControllerBase
{
    /// <summary>
    /// Add a product to the catalog.
    /// </summary>
    /// <param name="addProduct">Payload holding the command.</param>
    /// <returns>Awaitable <see cref="Task"/>.</returns>
    [HttpPost("add-product")]
    public async Task AddProduct([FromBody] AddProduct addProduct)
    {
        var product = grainFactory.GetGrain<IProduct>(addProduct.SKU);
        await product.Register(addProduct.Name);
    }

    /// <summary>
    /// Gets all the products in the catalog.
    /// </summary>
    /// <returns>Collection of products.</returns>
    [HttpGet]
    public IQueryable<Product> AllProducts() => catalogQueries.All();

    [HttpGet("generate"), AspNetResult]
    public async Task Generate()
    {
        for (var i = 0; i < 100; i++)
        {
            var product = new Product
            {
                Id = $"Sku-{i}",
                IsRegistered = true
            };

            await collection.InsertOneAsync(product);
        }
    }
}
