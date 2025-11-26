// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cratis.Arc.EntityFrameworkCore;

/// <summary>
/// Interceptor for saving changes in a read-only DbContext.
/// </summary>
public sealed class ReadOnlySaveChangesInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("This DbContext is read-only and does not support saving changes.");

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        => throw new InvalidOperationException("This DbContext is read-only and does not support saving changes.");
}