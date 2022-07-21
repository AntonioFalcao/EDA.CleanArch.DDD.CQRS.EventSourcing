using Application.Abstractions.Projections;
using Application.Abstractions.UseCases;
using Contracts.Services.Account;

namespace Application.UseCases.AddBillingAddress;

public class ProjectBillingAddressAddedInteractor : IInteractor<DomainEvent.BillingAddressAdded>
{
    private readonly IProjectionRepository<Projection.Address> _repository;

    public ProjectBillingAddressAddedInteractor(IProjectionRepository<Projection.Address> repository)
    {
        _repository = repository;
    }

    public async Task InteractAsync(DomainEvent.BillingAddressAdded @event, CancellationToken ct)
    {
        Projection.BillingAddress address = new(@event.AddressId, @event.AccountId, @event.Address, false);
        await _repository.InsertAsync(address, ct);
    }
}