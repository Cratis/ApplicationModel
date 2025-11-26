// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.EntityFrameworkCore.Concepts.for_ConceptAsValueConverter.when_converting_from_concept;

public class with_guid_concept : given.a_concept_as_value_converter
{
    TestGuidConcept _concept;
    Guid _expectedGuid;
    Guid _result;

    void Establish()
    {
        _expectedGuid = Guid.NewGuid();
        _concept = new TestGuidConcept(_expectedGuid);
    }

    void Because() => _result = (Guid)_guidConverter.ConvertToProvider(_concept)!;

    [Fact] void should_extract_the_underlying_guid_value() => _result.ShouldEqual(_expectedGuid);
}
