// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsValueComparer.when_getting_hash_code;

public class with_int_concept : given.a_concept_as_value_comparer
{
    TestIntConcept _concept;
    int _result;

    void Establish()
    {
        _concept = new TestIntConcept(42);
    }

    void Because() => _result = _intComparer.GetHashCode(_concept);

    [Fact] void should_return_hash_code_of_underlying_value() => _result.ShouldEqual(42.GetHashCode());
}
