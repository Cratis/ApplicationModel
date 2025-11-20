// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsValueComparer.when_comparing;

public class with_different_int_concepts : given.a_concept_as_value_comparer
{
    TestIntConcept _left;
    TestIntConcept _right;
    bool _result;

    void Establish()
    {
        _left = new TestIntConcept(42);
        _right = new TestIntConcept(84);
    }

    void Because() => _result = _intComparer.Equals(_left, _right);

    [Fact] void should_not_be_equal() => _result.ShouldBeFalse();
}
