// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Mapping;

/// <summary>
/// Defines a system for registering entity maps.
/// </summary>
public interface IEntityTypeRegistrar
{
    /// <summary>
    /// Register entity maps for the given model builder.
    /// </summary>
    /// <param name="dbContext"><see cref="DbContext"/> the model builder belongs to.</param>
    /// <param name="modelBuilder"><see cref="ModelBuilder"/> to register maps for.</param>
    void RegisterEntityMaps(DbContext dbContext, ModelBuilder modelBuilder);
}
