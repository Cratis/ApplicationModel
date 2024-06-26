// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Orleans.StateMachines.when_asking_can_transition_to;

public class and_state_does_not_allow_it : given.a_state_machine_with_well_known_states
{
    bool result;

    protected override Type initial_state => typeof(StateThatDoesNotSupportTransitioningFrom);

    async Task Because() => result = await state_machine.CanTransitionTo<StateThatSupportsTransitioningFrom>();

    [Fact] void should_not_be_able_to_transition() => result.ShouldBeFalse();
}
