// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Orleans.StateMachines.when_activating;

public class with_unknown_initial_state_type : given.a_state_machine
{
    Exception exception;


    protected override Type initial_state => typeof(StateThatDoesNotSupportTransitioningFrom);

    protected override IEnumerable<IState<StateMachineStateForTesting>> CreateStates() =>
    [
        new StateThatSupportsTransitioningFrom()
    ];

    void Because() => exception = Catch.Exception(() => _ = state_machine);

    [Fact] void should_throw_invalid_type_for_state_exception() => exception.ShouldBeOfExactType<UnknownStateTypeInStateMachine>();
}
