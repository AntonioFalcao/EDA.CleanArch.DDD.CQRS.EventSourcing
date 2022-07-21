using Application.Abstractions.UseCases;
using Contracts.Services.Account;
using Infrastructure.MessageBus.Abstractions;

namespace Infrastructure.MessageBus.Consumers.Events;

public class BillingAddressAddedConsumer : Consumer<DomainEvent.BillingAddressAdded>
{
    public BillingAddressAddedConsumer(IInteractor<DomainEvent.BillingAddressAdded> interactor)
        : base(interactor) { }
}