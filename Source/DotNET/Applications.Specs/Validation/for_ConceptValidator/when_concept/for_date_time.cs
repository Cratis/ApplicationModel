// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Cratis.Applications.Validation.for_ConceptValidator.when_concept;

public class for_date_time : Specification
{
    class validator : ConceptValidator<date_time_concept>
    {
        public validator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
    validator the_validator;

    void Because() => the_validator = new validator();

    [Fact]
    void should_not_fail_when_validating_non_empty_value() => the_validator.Validate(new date_time_concept(DateTime.MaxValue)).IsValid.ShouldBeTrue();

    [Fact]
    void should_fail_when_validating_empty_value() => the_validator.Validate(new date_time_concept(default)).IsValid.ShouldBeFalse();
}