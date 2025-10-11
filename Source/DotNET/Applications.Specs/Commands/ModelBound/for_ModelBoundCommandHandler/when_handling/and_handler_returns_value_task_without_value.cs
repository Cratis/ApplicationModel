// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Commands.ModelBound.for_ModelBoundCommandHandler.when_handling;

public class and_handler_returns_value_task_without_value : Specification
{
    record Command()
    {
        public IEnumerable<object> Dependencies;
        public ValueTask Handle(int firstDependency, string secondDependency)
        {
            Dependencies = [firstDependency, secondDependency];
            return ValueTask.CompletedTask;
        }
    }

    ModelBoundCommandHandler _handler;
    object _result;
    CommandContext _context;
    object[] _dependencies = [42, "The answer to life, the universe and everything"];

    void Establish()
    {
        _context = new(CorrelationId.New(), typeof(Command), new Command(), _dependencies, new());
        _handler = new ModelBoundCommandHandler(
            typeof(Command),
            typeof(Command).GetMethod(nameof(Command.Handle))!);
    }

    async Task Because() => _result = await _handler.Handle(_context);

    [Fact] void should_return_null() => _result.ShouldBeNull();
    [Fact] void should_pass_dependencies_to_handler() => ((Command)_context.Command).Dependencies.ShouldEqual(_dependencies);
}
