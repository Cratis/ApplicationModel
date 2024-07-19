// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Formats.Asn1;

namespace Cratis.Applications.Orleans.StateMachines.when_activating;

public class with_known_current_state : given.a_state_machine
{
    StateMachineStateForTesting on_enter_called_state;

    protected override Type initial_state => typeof(StateThatDoesNotSupportTransitioningFrom);

    protected override IEnumerable<IState<StateMachineStateForTesting>> CreateStates() =>
    [
        new StateThatDoesNotSupportTransitioningFrom(),
        new StateThatSupportsTransitioningFrom { OnEnterCalled = _ => on_enter_called_state = _ }
    ];

    void Establish()
    {
        // We get the state machine so that it calls the OnActivateAsync method before we set the state to test for
        _ = state_machine;
        state_storage.State = new StateMachineStateForTesting { CurrentState = typeof(StateThatSupportsTransitioningFrom).FullName };
    }

    async Task Because() => await state_machine.OnActivateAsync(CancellationToken.None);

    [Fact] async Task should_set_current_state_to_expected_initial_state_type() => (await state_machine.GetCurrentState()).ShouldBeAssignableFrom<StateThatSupportsTransitioningFrom>();
    [Fact] void should_call_on_enter_with_expected_state() => on_enter_called_state.ShouldEqual(state_storage.State);
    [Fact] async Task should_set_state_machine_on_all_states() => (await state_machine.GetStates()).All(_ => _.StateMachine.Equals(state_machine)).ShouldBeTrue();
}
