using Application.Abstractions.Projections;
using Contracts.Services.Account;
using MassTransit;

namespace Application.UseCases.Events.Projections;

public class ProjectAccountWhenChangedConsumer :
    IConsumer<DomainEvent.AccountCreated>,
    IConsumer<DomainEvent.AccountDeleted>
{
    private readonly IProjectionRepository<Projection.Account> _repository;

    public ProjectAccountWhenChangedConsumer(IProjectionRepository<Projection.Account> repository)
        => _repository = repository;

    public Task Consume(ConsumeContext<DomainEvent.AccountCreated> context)
    {
        Projection.Account account = new(context.Message.AccountId, context.Message.Profile, false);
        return _repository.InsertAsync(account, context.CancellationToken);
    }

    public Task Consume(ConsumeContext<DomainEvent.AccountDeleted> context)
        => _repository.DeleteAsync(context.Message.AccountId, context.CancellationToken);
}