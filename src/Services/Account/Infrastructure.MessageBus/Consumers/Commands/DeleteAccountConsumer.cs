using Application.Abstractions.UseCases;
using Contracts.Services.Account;
using Infrastructure.MessageBus.Abstractions;

namespace Infrastructure.MessageBus.Consumers.Commands;

public class DeleteAccountConsumer : Consumer<Command.DeleteAccount>
{
    public DeleteAccountConsumer(IInteractor<Command.DeleteAccount> interactor) 
        : base(interactor) { }
}