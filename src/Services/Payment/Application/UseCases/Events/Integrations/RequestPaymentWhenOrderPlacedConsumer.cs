﻿using Application.EventStore;
using Domain.Aggregates;
using ECommerce.Contracts.Payments;
using MassTransit;
using DomainEvent = ECommerce.Contracts.Orders.DomainEvent;

namespace Application.UseCases.Events.Integrations;

public class RequestPaymentWhenOrderPlacedConsumer : IConsumer<DomainEvent.OrderPlaced>
{
    private readonly IPaymentEventStoreService _eventStore;

    public RequestPaymentWhenOrderPlacedConsumer(IPaymentEventStoreService eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Consume(ConsumeContext<DomainEvent.OrderPlaced> context)
    {
        var payment = new Payment();

        payment.Handle(new Command.RequestPayment(
            context.Message.OrderId,
            context.Message.Total,
            context.Message.Customer.BillingAddress,
            context.Message.PaymentMethods));

        await _eventStore.AppendEventsAsync(payment, context.CancellationToken);
    }
}