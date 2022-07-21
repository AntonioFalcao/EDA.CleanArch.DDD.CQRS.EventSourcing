using Application.Abstractions.UseCases;
using Application.EventStore;
using Contracts.Services.Account;

namespace Application.UseCases.DeleteAccount;

public class DeleteAccountInteractor : IInteractor<Command.DeleteAccount>
{
    private readonly IAccountEventStoreService _eventStore;

    public DeleteAccountInteractor(IAccountEventStoreService eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task InteractAsync(Command.DeleteAccount command, CancellationToken ct)
    {
        var account = await _eventStore.LoadAggregateAsync(command.AccountId, ct);
        account.Handle(command);
        await _eventStore.AppendEventsAsync(account, ct);
    }
}