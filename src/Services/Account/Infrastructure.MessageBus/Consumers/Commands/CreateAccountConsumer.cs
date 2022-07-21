using Application.Abstractions.UseCases;
using Contracts.Services.Account;
using Infrastructure.MessageBus.Abstractions;

namespace Infrastructure.MessageBus.Consumers.Commands;

public class CreateAccountConsumer : Consumer<Command.CreateAccount>
{
    public CreateAccountConsumer(IInteractor<Command.CreateAccount> interactor) 
        : base(interactor) { }
}