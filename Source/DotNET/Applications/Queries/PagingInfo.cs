// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents the result coming from performing a query with a collection of items.
/// </summary>
/// <param name="Page">The page number.</param>
/// <param name="Size">The size of the page.</param>
/// <param name="TotalItems">The total number of items.</param>
/// <param name="TotalPages">The total number of pages.</param>
public record PagingInfo(int Page, int Size, int TotalItems, int TotalPages)
{
    /// <summary>
    /// Represents a not paged result.
    /// </summary>
    public static readonly PagingInfo NotPaged = new(0, 0, 0, 0);
}
