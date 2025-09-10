// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Commands.ModelBound.for_ModelBoundCommandHandler.when_handling;

public class and_handler_returns_a_task : Specification
{
    record Command()
    {
        public Guid Expected = Guid.NewGuid();
        public Task<Guid> Handle() => Task.FromResult(Expected);
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

    [Fact] void should_return_correct_type() => _result.ShouldBeOfExactType<Guid>();
    [Fact] void should_return_expected_value() => _result.ShouldEqual(((Command)_context.Command).Expected);
}
