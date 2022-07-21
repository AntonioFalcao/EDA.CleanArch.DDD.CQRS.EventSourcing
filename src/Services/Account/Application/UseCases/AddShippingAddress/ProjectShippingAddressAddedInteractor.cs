using Application.Abstractions.Projections;
using Application.Abstractions.UseCases;
using Contracts.Services.Account;

namespace Application.UseCases.AddShippingAddress;

public class ProjectShippingAddressAddedInteractor : IInteractor<DomainEvent.ShippingAddressAdded>
{
    private readonly IProjectionRepository<Projection.Address> _repository;

    public ProjectShippingAddressAddedInteractor(IProjectionRepository<Projection.Address> repository)
    {
        _repository = repository;
    }

    public async Task InteractAsync(DomainEvent.ShippingAddressAdded @event, CancellationToken ct)
    {
        Projection.ShippingAddress address = new(@event.AddressId, @event.AccountId, @event.Address, false);
        await _repository.InsertAsync(address, ct);
    }
}