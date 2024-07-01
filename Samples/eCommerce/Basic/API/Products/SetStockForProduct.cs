// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Concepts;

namespace API.Products;

/// <summary>
/// Represent the command for setting stock for a product.
/// </summary>
/// <param name="Quantity">Actual quantity.</param>
public record SetStockForProduct(Quantity Quantity);