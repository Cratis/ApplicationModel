// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.EntityFrameworkCore.Concepts.for_ConceptAsValueConverter.when_converting_from_concept;

public class with_int_concept : given.a_concept_as_value_converter
{
    TestIntConcept _concept;
    int _result;

    void Establish()
    {
        _concept = new TestIntConcept(42);
    }

    void Because() => _result = (int)_intConverter.ConvertToProvider(_concept)!;

    [Fact] void should_extract_the_underlying_int_value() => _result.ShouldEqual(42);
}
