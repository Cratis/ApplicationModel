// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Orleans.StateMachines.when_activating;

public class with_invalid_initial_state_type : given.a_state_machine
{
    Exception _exception;

    protected override Type InitialState => typeof(string);
    protected override IEnumerable<IState<StateMachineStateForTesting>> CreateStates() => [];

    async Task Because() => _exception = await Catch.Exception(async () => await StateMachine.OnActivateAsync(CancellationToken.None));

    [Fact] void should_throw_invalid_type_for_state_exception() => _exception.ShouldBeOfExactType<InvalidTypeForState>();
}
