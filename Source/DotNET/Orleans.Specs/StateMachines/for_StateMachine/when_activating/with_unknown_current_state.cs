// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Orleans.StateMachines.when_activating;

public class with_unknown_current_state : given.a_state_machine
{
    Exception exception;

    protected override IEnumerable<IState<StateMachineStateForTesting>> CreateStates() => new[] { new StateThatSupportsTransitioningFrom() };

    void Establish()
    {
        // We get the state machine so that it calls the OnActivateAsync method before we set the state to test for
        _ = state_machine;
        state_storage.State = new StateMachineStateForTesting { CurrentState = "Something" };
    }

    async Task Because() => exception = await Catch.Exception(async () => await state_machine.OnActivateAsync(CancellationToken.None));

    [Fact] void should_throw_invalid_type_for_state_exception() => exception.ShouldBeOfExactType<UnknownCurrentState>();
}
