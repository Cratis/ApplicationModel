// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsEvaluatableExpressionFilter;

#pragma warning disable SA1402 // Single type per file
#pragma warning disable SA1649 // File name should match first type name

public record TestIdConcept(Guid Value) : ConceptAs<Guid>(Value)
{
    public static readonly TestIdConcept NotSet = new(Guid.Empty);
    public static implicit operator TestIdConcept(Guid value) => new(value);
    public static implicit operator Guid(TestIdConcept concept) => concept.Value;
}

public class TestEntity
{
    public TestIdConcept Id { get; set; } = TestIdConcept.NotSet;
    public string Name { get; set; } = string.Empty;
}

public class TestClosure
{
    public TestIdConcept Id { get; set; } = TestIdConcept.NotSet;
}

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestEntity> TestEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);

        // Apply concept converters
        modelBuilder.Entity<TestEntity>()
            .Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => new TestIdConcept(v));
    }
}
