// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Commands.for_CommandPipeline.when_executing;

public class and_handler_returns_a_tuple_with_response_and_value : given.a_command_pipeline_and_a_handler_for_command
{
    CommandResult<string> _result;
    (string, int) _tuple;
    string _errorMessage;

    void Establish()
    {
        _tuple = ("Forty two", 42);
        _errorMessage = Guid.NewGuid().ToString();
        _commandHandler.Handle(Arg.Any<CommandContext>()).Returns(_tuple);
        _commandResponseValueHandlers.Handle(Arg.Any<CommandContext>(), _tuple.Item2).Returns(CommandResult.Error(CorrelationId.New(), _errorMessage));
    }

    async Task Because() => _result = (await _commandPipeline.Execute(_command)) as CommandResult<string>;

    [Fact] void should_call_value_handlers() => _commandResponseValueHandlers.Received(1).Handle(Arg.Any<CommandContext>(), _tuple.Item2);
    [Fact] void should_return_response_in_command_result() => _result.Response.ShouldEqual(_tuple.Item1);
    [Fact] void should_return_error_from_value_handlers() => _result.ExceptionMessages.First().ShouldEqual(_errorMessage);
}
