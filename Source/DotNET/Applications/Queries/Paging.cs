// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries;

/// <summary>
/// Represents paging for a query.
/// </summary>
/// <param name="Page">The page number.</param>
/// <param name="Size">The size of a page.</param>
/// <param name="IsPaged">Whether or not paging is to be used.</param>
public record Paging(int Page, int Size, bool IsPaged)
{
    /// <summary>
    /// Represents a not paged result.
    /// </summary>
    public static readonly Paging NotPaged = new(0, 0, false);
}
