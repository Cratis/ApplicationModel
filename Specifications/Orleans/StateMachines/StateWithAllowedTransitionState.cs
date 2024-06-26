// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace Cratis.Applications.Orleans.StateMachines;

public class StateWithAllowedTransitionState : BaseState
{
    protected override IImmutableList<Type> AllowedTransitions => new[] { typeof(StateThatSupportsTransitioningFrom) }.ToImmutableList();
}
