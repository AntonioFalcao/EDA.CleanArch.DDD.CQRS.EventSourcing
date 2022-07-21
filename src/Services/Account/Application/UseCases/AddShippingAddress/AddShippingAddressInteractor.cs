using Application.Abstractions.UseCases;
using Application.EventStore;
using Contracts.Services.Account;

namespace Application.UseCases.AddShippingAddress;

public class AddShippingAddressInteractor : IInteractor<Command.AddShippingAddress>
{
    private readonly IAccountEventStoreService _eventStore;

    public AddShippingAddressInteractor(IAccountEventStoreService eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task InteractAsync(Command.AddShippingAddress command, CancellationToken ct)
    {
        var account = await _eventStore.LoadAggregateAsync(command.AccountId, ct);
        account.Handle(command);
        await _eventStore.AppendEventsAsync(account, ct);
    }
}