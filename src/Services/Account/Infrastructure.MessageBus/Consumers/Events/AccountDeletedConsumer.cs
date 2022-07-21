using Application.Abstractions.UseCases;
using Contracts.Services.Account;
using Infrastructure.MessageBus.Abstractions;

namespace Infrastructure.MessageBus.Consumers.Events;

public class AccountDeletedConsumer : Consumer<DomainEvent.AccountDeleted>
{
    public AccountDeletedConsumer(IInteractor<DomainEvent.AccountDeleted> interactor)
        : base(interactor) { }
}