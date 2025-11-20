// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsValueComparer.when_getting_hash_code;

public class with_string_concept : given.a_concept_as_value_comparer
{
    TestStringConcept _concept;
    int _result;

    void Establish()
    {
        _concept = new TestStringConcept("test value");
    }

    void Because() => _result = _stringComparer.GetHashCode(_concept);

    [Fact] void should_return_hash_code_of_underlying_value() => _result.ShouldEqual("test value".GetHashCode());
}
