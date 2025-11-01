// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar;

#pragma warning disable SA1402, SA1649 // Single type per file,  File name should match first type name
public class TestEntityMap : IEntityTypeConfiguration<TestEntity>
{
    public virtual void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100);
    }
}

public class AnotherTestEntityMap : IEntityTypeConfiguration<AnotherTestEntity>
{
    public virtual void Configure(EntityTypeBuilder<AnotherTestEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Description).HasMaxLength(200);
    }
}
#pragma warning restore SA1402, SA1649 // Single type per file,  File name should match first type name