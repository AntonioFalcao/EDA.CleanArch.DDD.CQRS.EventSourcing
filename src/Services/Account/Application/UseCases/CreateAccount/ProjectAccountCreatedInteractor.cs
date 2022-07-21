using Application.Abstractions.Projections;
using Application.Abstractions.UseCases;
using Contracts.Services.Account;

namespace Application.UseCases.CreateAccount;

public class ProjectAccountCreatedInteractor : IInteractor<DomainEvent.AccountCreated>
{
    private readonly IProjectionRepository<Projection.Account> _repository;

    public ProjectAccountCreatedInteractor(IProjectionRepository<Projection.Account> repository)
    {
        _repository = repository;
    }

    public Task InteractAsync(DomainEvent.AccountCreated @event, CancellationToken ct)
        => _repository.InsertAsync(new(@event.AccountId, @event.Profile, false), ct);
}