// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<AnotherTestEntity> AnotherTestEntities { get; set; }
    public DbSet<EntityWithoutMap> EntitiesWithoutMap { get; set; }
}