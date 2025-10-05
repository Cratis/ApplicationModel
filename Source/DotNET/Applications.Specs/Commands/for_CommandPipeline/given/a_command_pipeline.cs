// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Commands.for_CommandPipeline.given;

public class a_command_pipeline : Specification
{
    protected ICorrelationIdAccessor _correlationIdAccessor;
    protected ICommandFilters _commandFilters;
    protected ICommandHandlerProviders _commandHandlerProviders;
    protected ICommandResponseValueHandlers _commandResponseValueHandlers;
    protected ICommandContextModifier _commandContextModifier;
    protected ICommandContextValuesBuilder _commandContextValuesBuilder;
    protected IServiceProvider _serviceProvider;
    protected CommandPipeline _commandPipeline;

    void Establish()
    {
        var correlationId = CorrelationId.New();
        _correlationIdAccessor = Substitute.For<ICorrelationIdAccessor>();
        _correlationIdAccessor.Current.Returns(correlationId);
        _commandFilters = Substitute.For<ICommandFilters>();
        _commandFilters.OnExecution(Arg.Any<CommandContext>()).Returns(CommandResult.Success(correlationId));
        _commandHandlerProviders = Substitute.For<ICommandHandlerProviders>();
        _commandResponseValueHandlers = Substitute.For<ICommandResponseValueHandlers>();
        _commandContextModifier = Substitute.For<ICommandContextModifier>();
        _commandContextValuesBuilder = Substitute.For<ICommandContextValuesBuilder>();
        _commandContextValuesBuilder.Build().Returns(new CommandContextValues());
        _serviceProvider = Substitute.For<IServiceProvider>();

        _commandPipeline = new(
            _correlationIdAccessor,
            _commandFilters,
            _commandHandlerProviders,
            _commandResponseValueHandlers,
            _commandContextModifier,
            _commandContextValuesBuilder,
            _serviceProvider);
    }
}
