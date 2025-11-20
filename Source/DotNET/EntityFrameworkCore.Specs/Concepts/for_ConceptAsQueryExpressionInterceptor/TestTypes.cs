// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsQueryExpressionInterceptor;

#pragma warning disable SA1402 // Single type per file

public record TestIdConcept(Guid Value) : ConceptAs<Guid>(Value)
{
    public static readonly TestIdConcept NotSet = new(Guid.Empty);
    public static implicit operator TestIdConcept(Guid value) => new(value);
}

public class TestEntity
{
    public TestIdConcept Id { get; set; } = TestIdConcept.NotSet;
    public string Name { get; set; } = string.Empty;
}
