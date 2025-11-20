// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsValueConverter.given;

public class a_concept_as_value_converter : Specification
{
    protected ConceptAsValueConverter<TestStringConcept, string> _stringConverter;
    protected ConceptAsValueConverter<TestIntConcept, int> _intConverter;
    protected ConceptAsValueConverter<TestGuidConcept, Guid> _guidConverter;

    void Establish()
    {
        _stringConverter = new ConceptAsValueConverter<TestStringConcept, string>();
        _intConverter = new ConceptAsValueConverter<TestIntConcept, int>();
        _guidConverter = new ConceptAsValueConverter<TestGuidConcept, Guid>();
    }
}
