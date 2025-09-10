// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Commands.for_CommandPipeline.when_executing;

public class and_there_is_a_handler_that_has_dependencies : given.a_command_pipeline
{
    string _command;
    CommandResult _result;
    ICommandHandler _commandHandler;

    void Establish()
    {
        _command = Guid.NewGuid().ToString();
        _commandHandler = Substitute.For<ICommandHandler>();
        var anyHandler = Arg.Any<ICommandHandler>();
        _commandHandlerProviders
            .TryGetHandlerFor(_command, out anyHandler)
            .Returns(r =>
            {
                r[1] = _commandHandler;
                return true;
            });
    }

    async Task Because() => _result = await _commandPipeline.Execute(_command);

    [Fact] void should_call_command_handler() => _commandHandler.Received(1).Handle(Arg.Any<CommandContext>());
}
