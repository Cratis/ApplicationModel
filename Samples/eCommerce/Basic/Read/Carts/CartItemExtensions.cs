// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Concepts;
using Read.Orders;

namespace Read.Carts;

/// <summary>
/// Extensions for <see cref="CartItem"/>.
/// </summary>
public static class CartItemExtensions
{
    /// <summary>
    /// Convert a collection of <see cref="CartItem"/> to <see cref="OrderLine"/>.
    /// </summary>
    /// <param name="cartItems">Cart items to convert.</param>
    /// <returns>Converted order lines.</returns>
    public static IEnumerable<OrderLine> ToOrderLines(this IEnumerable<CartItem> cartItems)
    {
        return [.. cartItems.Select(cartItem => new OrderLine(cartItem.SKU, cartItem.Price, cartItem.Quantity))];
    }
}