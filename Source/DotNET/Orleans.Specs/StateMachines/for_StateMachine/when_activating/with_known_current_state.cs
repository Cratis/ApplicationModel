// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Orleans.StateMachines.when_activating;

public class with_known_current_state : given.a_state_machine
{
    StateMachineStateForTesting _onEnterCalledState;

    protected override Type InitialState => typeof(StateThatDoesNotSupportTransitioningFrom);

    protected override IEnumerable<IState<StateMachineStateForTesting>> CreateStates() =>
    [
        new StateThatDoesNotSupportTransitioningFrom(),
        new StateThatSupportsTransitioningFrom { OnEnterCalled = _ => _onEnterCalledState = _ }
    ];

    void Establish()
    {
        // We get the state machine so that it calls the OnActivateAsync method before we set the state to test for
        _ = StateMachine;
        _stateStorage.State = new StateMachineStateForTesting { CurrentState = typeof(StateThatSupportsTransitioningFrom).FullName };
    }

    async Task Because() => await StateMachine.OnActivateAsync(CancellationToken.None);

    [Fact] async Task should_set_current_state_to_expected_initial_state_type() => (await StateMachine.GetCurrentState()).ShouldBeAssignableFrom<StateThatSupportsTransitioningFrom>();
    [Fact] void should_call_on_enter_with_expected_state() => _onEnterCalledState.ShouldEqual(_stateStorage.State);
    [Fact] async Task should_set_state_machine_on_all_states() => (await StateMachine.GetStates()).All(_ => _.StateMachine.Equals(StateMachine)).ShouldBeTrue();
}
