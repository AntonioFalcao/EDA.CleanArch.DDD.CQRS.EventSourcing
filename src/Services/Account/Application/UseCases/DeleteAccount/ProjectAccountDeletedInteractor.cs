using Application.Abstractions.Projections;
using Application.Abstractions.UseCases;
using Contracts.Services.Account;

namespace Application.UseCases.DeleteAccount;

public class ProjectAccountDeletedInteractor : IInteractor<DomainEvent.AccountDeleted>
{
    private readonly IProjectionRepository<Projection.Account> _repository;

    public ProjectAccountDeletedInteractor(IProjectionRepository<Projection.Account> repository)
    {
        _repository = repository;
    }

    public Task InteractAsync(DomainEvent.AccountDeleted @event, CancellationToken ct)
        => _repository.DeleteAsync(@event.AccountId, ct);
}