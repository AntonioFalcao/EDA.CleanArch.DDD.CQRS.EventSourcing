using Application.EventStore;
using Application.EventStore.Events;
using Domain.Aggregates;
using Infrastructure.EventStore.Abstractions;
using Infrastructure.EventStore.Contexts;

namespace Infrastructure.EventStore;

public class AccountEventStoreRepository : EventStoreRepository<Account, AccountStoreEvent, AccountSnapshot, Guid>, IAccountEventStoreRepository
{
    public AccountEventStoreRepository(EventStoreDbContext dbContext)
        : base(dbContext) { }
}