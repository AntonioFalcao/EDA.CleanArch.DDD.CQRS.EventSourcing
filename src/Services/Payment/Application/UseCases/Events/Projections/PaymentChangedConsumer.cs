﻿using System.Threading.Tasks;
using Application.EventSourcing.EventStore;
using Application.EventSourcing.Projections;
using MassTransit;
using PaymentCanceledEvent = Messages.Services.Payments.DomainEvents.PaymentCanceled;

namespace Application.UseCases.Events.Projections;

public class PaymentChangedConsumer : IConsumer<PaymentCanceledEvent>
{
    private readonly IPaymentEventStoreService _eventStoreService;
    private readonly IPaymentProjectionsService _projectionsService;

    public PaymentChangedConsumer(IPaymentEventStoreService eventStoreService, IPaymentProjectionsService projectionsService)
    {
        _eventStoreService = eventStoreService;
        _projectionsService = projectionsService;
    }

    public async Task Consume(ConsumeContext<PaymentCanceledEvent> context)
    {
        var payment = await _eventStoreService.LoadAggregateFromStreamAsync(context.Message.PaymentId, context.CancellationToken);

        var paymentDetails = new PaymentDetailsProjection
        {
            Id = payment.Id,
            IsDeleted = payment.IsDeleted,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status.ToString(),
            BillingAddressProjection = payment.BillingAddress is null
                ? default
                : new()
                {
                    City = payment.BillingAddress.City,
                    Country = payment.BillingAddress.Country,
                    Number = payment.BillingAddress.Number,
                    State = payment.BillingAddress.State,
                    Street = payment.BillingAddress.Street,
                    ZipCode = payment.BillingAddress.ZipCode
                },
            CreditCardProjection = payment.CreditCard is null
                ? default
                : new()
                {
                    Expiration = payment.CreditCard.Expiration,
                    Number = payment.CreditCard.Number,
                    HolderName = payment.CreditCard.HolderName,
                    SecurityNumber = payment.CreditCard.SecurityNumber
                }
        };

        await _projectionsService.UpdatePaymentDetailsAsync(paymentDetails, context.CancellationToken);
    }
}