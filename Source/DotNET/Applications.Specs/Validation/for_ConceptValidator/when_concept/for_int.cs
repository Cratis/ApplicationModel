using FluentValidation;

namespace Cratis.Applications.Validation.for_ConceptValidator.when_concept;

public class for_int : Specification
{
    class validator : ConceptValidator<int_concept>
    {
        public validator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
    validator the_validator;

    void Because() => the_validator = new validator();

    [Fact]
    void should_not_fail_when_validating_non_empty_value() => the_validator.Validate(new int_concept(short.MaxValue)).IsValid.ShouldBeTrue();

    [Fact]
    void should_fail_when_validating_empty_value() => the_validator.Validate(new int_concept(default)).IsValid.ShouldBeFalse();
}