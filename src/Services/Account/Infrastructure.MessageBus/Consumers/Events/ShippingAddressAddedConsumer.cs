using Application.Abstractions.UseCases;
using Contracts.Services.Account;
using Infrastructure.MessageBus.Abstractions;

namespace Infrastructure.MessageBus.Consumers.Events;

public class ShippingAddressAddedConsumer : Consumer<DomainEvent.ShippingAddressAdded>
{
    public ShippingAddressAddedConsumer(IInteractor<DomainEvent.ShippingAddressAdded> interactor)
        : base(interactor) { }
}