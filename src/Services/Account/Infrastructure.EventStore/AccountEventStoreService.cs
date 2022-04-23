using Application.Abstractions.Notifications;
using Application.EventStore;
using Application.EventStore.Events;
using Domain.Aggregates;
using Infrastructure.EventStore.Abstractions;
using Infrastructure.EventStore.DependencyInjection.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Infrastructure.EventStore;

public class AccountEventStoreService : EventStoreService<Account, AccountStoreEvent, AccountSnapshot, Guid>, IAccountEventStoreService
{
    public AccountEventStoreService(IPublishEndpoint publishEndpoint, IAccountEventStoreRepository repository, INotificationContext notificationContext, IOptionsMonitor<EventStoreOptions> optionsMonitor)
        : base(publishEndpoint, repository, notificationContext, optionsMonitor) { }
}