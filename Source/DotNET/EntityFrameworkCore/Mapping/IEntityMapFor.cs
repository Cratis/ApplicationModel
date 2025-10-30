// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Defines a map for mapping a type to an entity for Entity Framework Core.
/// </summary>
/// <typeparam name="T">Type the entity map is for.</typeparam>
public interface IEntityMapFor<T>
    where T : class
{
    /// <summary>
    /// Configure the given entity type builder.
    /// </summary>
    /// <param name="builder"><see cref="EntityTypeBuilder{T}"/> to configure.</param>
    void Configure(EntityTypeBuilder<T> builder);
}
