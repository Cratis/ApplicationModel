// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Orleans.StateMachines.for_State.when_asking_can_transition;

public class and_it_has_state_as_allowed : Specification
{
    bool result;
    StateWithAllowedTransitionState state;

    void Establish() => state = new();

    async Task Because() => result = await state.CanTransitionTo<StateThatSupportsTransitioningFrom>(null!);

    [Fact] void should_return_true() => result.ShouldBeTrue();
}
