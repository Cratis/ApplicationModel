// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Commands.ModelBound.for_ModelBoundCommandHandler.when_handling;

public class and_handler_returns_void : Specification
{
    record Command()
    {
        public void Handle() { }
    }

    ModelBoundCommandHandler _handler;
    object _result;
    CommandContext _context;

    void Establish()
    {
        _context = new(CorrelationId.New(), typeof(Command), new Command(), []);
        _handler = new ModelBoundCommandHandler(
            typeof(Command),
            typeof(Command).GetMethod(nameof(Command.Handle))!);
    }

    async Task Because() => _result = await _handler.Handle(_context);

    [Fact] void should_return_null() => _result.ShouldBeNull();
}
