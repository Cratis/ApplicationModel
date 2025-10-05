// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Cratis.Applications.Commands.Filters.for_FluentValidationFilter.when_executing_command;

public class with_validator_found_and_validation_succeeds : given.a_fluent_validation_filter
{
    CommandResult _result;
    IValidator _validator;
    FluentValidationResult _validationResult;
    SimpleCommand _command;

    void Establish()
    {
        _command = new SimpleCommand();
        _context = new CommandContext(_correlationId, typeof(SimpleCommand), _command, [], new());

        _validator = Substitute.For<IValidator>();
        _validationResult = new FluentValidationResult();

        _discoverableValidators.TryGet(typeof(SimpleCommand), out Arg.Any<IValidator>())
            .Returns(x =>
            {
                x[1] = _validator;
                return true;
            });

        _validator.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>()).Returns(_validationResult);
    }

    async Task Because() => _result = await _filter.OnExecution(_context);

    [Fact] void should_return_successful_result() => _result.IsSuccess.ShouldBeTrue();
    [Fact] void should_have_correlation_id() => _result.CorrelationId.ShouldEqual(_correlationId);
    [Fact] void should_be_authorized() => _result.IsAuthorized.ShouldBeTrue();
    [Fact] void should_be_valid() => _result.IsValid.ShouldBeTrue();
    [Fact] void should_not_have_exceptions() => _result.HasExceptions.ShouldBeFalse();
    [Fact] void should_not_have_validation_results() => _result.ValidationResults.ShouldBeEmpty();
    [Fact] void should_call_validator() => _validator.Received(1).ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>());

    public class SimpleCommand;
}