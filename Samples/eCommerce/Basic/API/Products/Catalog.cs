// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Domain.Products;
using Read.Products;

namespace API.Products;

/// <summary>
/// Represents the catalog API.
/// </summary>
/// <param name="grainFactory"><see cref="IGrainFactory"/> for working with grains.</param>
[Route("/api/products/catalog")]
public class Catalog(IGrainFactory grainFactory) : ControllerBase
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
    public Task<IQueryable<Product>> AllProducts()
    {
        return Task.FromResult(new List<Product>().AsQueryable());
    }
}
