using Application.Abstractions.UseCases;
using Contracts.Services.Account;
using Infrastructure.MessageBus.Abstractions;

namespace Infrastructure.MessageBus.Consumers.Commands;

public class AddShippingAddressConsumer : Consumer<Command.AddShippingAddress>
{
    public AddShippingAddressConsumer(IInteractor<Command.AddShippingAddress> interactor) 
        : base(interactor) { }
}