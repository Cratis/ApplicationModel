// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Commands;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Commands.ModelBound;

public class when_executing_simple_command : given.a_scenario_web_application
{
    CommandResult<object>? _result;

    async Task Because()
    {
        var payload = new
        {
            name = "TestName",
            value = 42
        };

        // SimpleCommand in Cratis.Arc.ProxyGenerator.Scenarios.Commands namespace
        var executionResult = await Bridge.ExecuteCommandDirectAsync(
            "/api/cratis/arc/proxy-generator/scenarios/commands/simple-command",
            payload);
        _result = executionResult.Result;
    }

    [Fact] void should_return_successful_result() => _result.IsSuccess.ShouldBeTrue();
    [Fact] void should_be_authorized() => _result.IsAuthorized.ShouldBeTrue();
    [Fact] void should_be_valid() => _result.IsValid.ShouldBeTrue();
    [Fact] void should_have_no_exceptions() => _result.HasExceptions.ShouldBeFalse();
    [Fact] void should_have_no_validation_results() => _result.ValidationResults.ShouldBeEmpty();
}
