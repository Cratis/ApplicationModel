// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityMapRegistrar;

public class EmptyDbContext(DbContextOptions<EmptyDbContext> options) : DbContext(options)
{
    // No DbSet properties
}