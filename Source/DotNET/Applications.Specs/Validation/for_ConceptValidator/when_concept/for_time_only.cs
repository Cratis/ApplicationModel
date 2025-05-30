using FluentValidation;

namespace Cratis.Applications.Validation.for_ConceptValidator.when_concept;

public class for_time_only : Specification
{
    class validator : ConceptValidator<time_only_concept>
    {
        public validator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
    validator the_validator;

    void Because() => the_validator = new validator();

    [Fact]
    void should_not_fail_when_validating_non_empty_value() => the_validator.Validate(new time_only_concept(TimeOnly.MaxValue)).IsValid.ShouldBeTrue();

    [Fact]
    void should_fail_when_validating_empty_value() => the_validator.Validate(new time_only_concept(default)).IsValid.ShouldBeFalse();
}