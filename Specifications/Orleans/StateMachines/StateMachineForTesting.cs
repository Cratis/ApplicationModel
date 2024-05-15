// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace Cratis.Kernel.Orleans.StateMachines;

public class StateMachineForTesting(IEnumerable<IState<StateMachineState>> states, Type? initialState = default) : StateMachine<StateMachineState>
{
    readonly Type _initialState = initialState;
    IImmutableList<IState<StateMachineState>> _states = states.ToImmutableList();

    public List<IState<StateMachineState>> OnBeforeEnteringStates { get; } = [];
    public List<IState<StateMachineState>> OnAfterEnteringStates { get; } = [];
    public List<IState<StateMachineState>> OnBeforeLeavingStates { get; } = [];
    public List<IState<StateMachineState>> OnAfterLeavingStates { get; } = [];


    protected override Type InitialState => _initialState ?? base.InitialState;

    public override IImmutableList<IState<StateMachineState>> CreateStates() => _states;

    protected override Task OnBeforeEnteringState(IState<StateMachineState> state)
    {
        if (state is NoOpState<StateMachineState>)
        {
            return Task.CompletedTask;
        }

        OnBeforeEnteringStates.Add(state);
        return Task.CompletedTask;
    }

    protected override Task OnAfterEnteringState(IState<StateMachineState> state)
    {
        if (state is NoOpState<StateMachineState>)
        {
            return Task.CompletedTask;
        }

        OnAfterEnteringStates.Add(state);
        return Task.CompletedTask;
    }

    protected override Task OnBeforeLeavingState(IState<StateMachineState> state)
    {
        if (state is NoOpState<StateMachineState>)
        {
            return Task.CompletedTask;
        }

        OnBeforeLeavingStates.Add(state);
        return Task.CompletedTask;
    }

    protected override Task OnAfterLeavingState(IState<StateMachineState> state)
    {
        if (state is NoOpState<StateMachineState>)
        {
            return Task.CompletedTask;
        }

        OnAfterLeavingStates.Add(state);
        return Task.CompletedTask;
    }
}