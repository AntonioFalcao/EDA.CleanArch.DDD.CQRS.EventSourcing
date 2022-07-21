using Application.Abstractions.UseCases;
using Contracts.Services.Account;
using Infrastructure.MessageBus.Abstractions;

namespace Infrastructure.MessageBus.Consumers.Events;

public class AccountCreatedConsumer : Consumer<DomainEvent.AccountCreated>
{
    public AccountCreatedConsumer(IInteractor<DomainEvent.AccountCreated> interactor)
        : base(interactor) { }
}