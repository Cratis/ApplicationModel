// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar;

public class TestEntityMap : IEntityMapFor<TestEntity>
{
    public virtual void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100);
    }
}

public class AnotherTestEntityMap : IEntityMapFor<AnotherTestEntity>
{
    public virtual void Configure(EntityTypeBuilder<AnotherTestEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Description).HasMaxLength(200);
    }
}