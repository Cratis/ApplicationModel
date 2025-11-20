// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsValueConverter;

#pragma warning disable SA1402 // Single type per file

public record TestStringConcept(string Value) : ConceptAs<string>(Value)
{
    public static readonly TestStringConcept Empty = new(string.Empty);
    public static implicit operator TestStringConcept(string value) => new(value);
}

public record TestIntConcept(int Value) : ConceptAs<int>(Value)
{
    public static readonly TestIntConcept NotSet = new(0);
    public static implicit operator TestIntConcept(int value) => new(value);
}

public record TestGuidConcept(Guid Value) : ConceptAs<Guid>(Value)
{
    public static readonly TestGuidConcept NotSet = new(Guid.Empty);
    public static implicit operator TestGuidConcept(Guid value) => new(value);
}
