// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Concepts;
using Cratis.Applications.EntityFrameworkCore.Json;
using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Base class for DbContexts that needs standard things configured.
/// </summary>
/// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
public class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyJsonConversion(Database);
        modelBuilder.ApplyConceptAsConversion();
        modelBuilder.ApplyGuidConversion(Database);
        base.OnModelCreating(modelBuilder);
    }
}
