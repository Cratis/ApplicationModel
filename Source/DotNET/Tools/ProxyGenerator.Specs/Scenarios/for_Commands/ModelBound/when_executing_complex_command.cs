// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Commands;
using Cratis.Arc.ProxyGenerator.Scenarios.Commands;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Commands.ModelBound;

public class when_executing_complex_command : given.a_scenario_web_application
{
    CommandResult<ComplexCommandResult>? _result;

    async Task Because()
    {
        var payload = new
        {
            nested = new
            {
                name = "NestedName",
                child = new
                {
                    id = Guid.NewGuid(),
                    value = 123.45
                }
            },
            items = new[] { "item1", "item2", "item3" },
            values = new Dictionary<string, int>
            {
                ["key1"] = 1,
                ["key2"] = 2
            }
        };

        // ComplexCommand in Cratis.Arc.ProxyGenerator.Scenarios.Commands namespace
        var executionResult = await Bridge!.ExecuteCommandDirectAsync<ComplexCommandResult>(
            "/api/cratis/arc/proxy-generator/scenarios/commands/complex-command",
            payload);
        _result = executionResult.Result;
    }

    [Fact] void should_return_successful_result() => _result!.IsSuccess.ShouldBeTrue();
    [Fact] void should_have_response() => _result!.Response.ShouldNotBeNull();
    [Fact] void should_have_received_nested_name() => _result!.Response!.ReceivedNested.ShouldEqual("NestedName");
    [Fact] void should_have_correct_item_count() => _result!.Response!.ItemCount.ShouldEqual(3);
    [Fact] void should_have_correct_value_count() => _result!.Response!.ValueCount.ShouldEqual(2);
}
