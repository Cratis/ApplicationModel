// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Chronicle.Events;

namespace Cratis.Arc.Chronicle.Aggregates.for_AggregateRootEventHandlers.when_creating_for;

public class AggregateRootWithHandlerMethodForEventType : AggregateRoot
{
    public Task Handle(MyEvent @event) => Task.CompletedTask;
}
