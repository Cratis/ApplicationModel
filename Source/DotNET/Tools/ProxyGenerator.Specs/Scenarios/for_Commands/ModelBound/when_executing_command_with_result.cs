// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Commands;
using Cratis.Arc.ProxyGenerator.Scenarios.Commands;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Commands.ModelBound;

public class when_executing_command_with_result : given.a_scenario_web_application
{
    CommandResult<CommandResultData>? _result;

    void Establish() => LoadCommandProxy<CommandWithResult>();

    async Task Because()
    {
        var properties = new Dictionary<string, object>
        {
            ["input"] = "TestInput"
        };

        var executionResult = await Bridge.ExecuteCommandViaProxyAsync<CommandResultData>(
            "CommandWithResult",
            properties);
        _result = executionResult.Result;
    }

    [Fact] void should_return_successful_result() => _result.IsSuccess.ShouldBeTrue();
    [Fact] void should_have_response() => _result.Response.ShouldNotBeNull();
    [Fact] void should_have_processed_value() => _result.Response.ProcessedValue.ShouldContain("Processed: TestInput");
}
