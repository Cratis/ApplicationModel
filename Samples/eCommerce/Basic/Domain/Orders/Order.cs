// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Xml;
using Concepts;
using Cratis.Applications.Orleans.StateMachines;
using Cratis.Collections;
using Domain.Orders.States;
using Read.Carts;

namespace Domain.Orders;

/// <summary>
/// Represents an implementation of <see cref="IOrder"/>.
/// </summary>
public class Order : StateMachine<Read.Orders.Order>, IOrder
{
    /// <inheritdoc/>
    public override IImmutableList<IState<Read.Orders.Order>> CreateStates() =>
    [
        new Initiated(),
        new DeliveryRecipientSet(),
        new InvoiceRecipientSet(),
        new Cancelled(),
        new PaymentAuthorized(),
        new Sent()
    ];

    /// <inheritdoc/>
    public async Task Initiate(Cart cart)
    {
        State.Lines = [.. cart.Items.ToOrderLines()];
        await TransitionTo<Initiated>();
    }

    /// <inheritdoc/>
    public async Task Cancel()
    {
        await TransitionTo<Cancelled>();
    }

    /// <inheritdoc/>
    public Task SetDeliveryRecipient(Recipient recipient)
    {
        State.DeliveryRecipient = recipient;
        return TransitionTo<DeliveryRecipientSet>();
    }

    /// <inheritdoc/>
    public Task SetInvoiceRecipient(Recipient recipient) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AuthorizePayment() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task PaymentAuthorized() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task Sent() => throw new NotImplementedException();
}
