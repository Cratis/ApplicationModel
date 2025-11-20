// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsValueComparer.when_comparing;

public class with_different_string_concepts : given.a_concept_as_value_comparer
{
    TestStringConcept _left;
    TestStringConcept _right;
    bool _result;

    void Establish()
    {
        _left = new TestStringConcept("value one");
        _right = new TestStringConcept("value two");
    }

    void Because() => _result = _stringComparer.Equals(_left, _right);

    [Fact] void should_not_be_equal() => _result.ShouldBeFalse();
}
