// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.EntityFrameworkCore.Concepts.for_ConceptAsValueComparer.given;

public class a_concept_as_value_comparer : Specification
{
    protected ConceptAsValueComparer<TestStringConcept, string> _stringComparer;
    protected ConceptAsValueComparer<TestIntConcept, int> _intComparer;

    void Establish()
    {
        _stringComparer = new ConceptAsValueComparer<TestStringConcept, string>(c => c.Value);
        _intComparer = new ConceptAsValueComparer<TestIntConcept, int>(c => c.Value);
    }
}
