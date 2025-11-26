// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
namespace Cratis.Arc.MongoDB.for_QueryContextAwareSet;

public record SomeId(Guid value) : ConceptAs<Guid>(value)
{
    public static implicit operator SomeId(Guid value) => new(value);
}