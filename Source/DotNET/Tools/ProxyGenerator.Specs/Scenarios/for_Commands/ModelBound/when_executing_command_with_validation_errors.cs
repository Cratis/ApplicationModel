// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Commands;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Commands.ModelBound;

public class when_executing_command_with_validation_errors : given.a_scenario_web_application
{
    CommandResult<object>? _result;

    void Establish() => LoadCommandProxy<ValidatedCommand>();

    async Task Because()
    {
        var properties = new Dictionary<string, object>
        {
            ["name"] = string.Empty, // Required field missing
            ["value"] = 150, // Out of range (1-100)
            ["email"] = "not-an-email" // Invalid email format
        };

        var executionResult = await Bridge.ExecuteCommandViaProxyAsync<object>(
            "ValidatedCommand",
            properties);
        _result = executionResult.Result;
    }

    [Fact] void should_not_be_successful() => _result.IsSuccess.ShouldBeFalse();
    [Fact] void should_not_be_valid() => _result.IsValid.ShouldBeFalse();
    [Fact] void should_have_validation_results() => _result.ValidationResults.ShouldNotBeEmpty();
}
