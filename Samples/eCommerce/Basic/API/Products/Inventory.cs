// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Concepts;

namespace API.Products;

/// <summary>
/// Represents the inventory API.
/// </summary>
public class Inventory : ControllerBase
{
    /// <summary>
    /// Set the stock for a product.
    /// </summary>
    /// <param name="sku"><see cref="SKU"/> representing the product.</param>
    /// <param name="command">The command payload.</param>
    /// <returns>Awaitable task.</returns>
    [HttpPost("set-stock/{sku}")]
    public Task SetStockForProduct([FromRoute] SKU sku, [FromBody] SetStockForProduct command)
    {
        Console.WriteLine($"Setting stock for product {sku} to {command.Quantity}");
        return Task.CompletedTask;
    }
}
