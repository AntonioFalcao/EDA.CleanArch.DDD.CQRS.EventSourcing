using Application.EventSourcing.EventStore;
using Application.EventSourcing.Projections;
using MassTransit;
using CatalogCreatedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogCreated;
using CatalogActivatedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogActivated;
using CatalogDeactivatedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogDeactivated;
using CatalogUpdatedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogUpdated;
using CatalogDeletedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogDeleted;
using CatalogItemAddedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogItemAdded;
using CatalogItemRemovedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogItemRemoved;
using CatalogItemUpdatedEvent = ECommerce.Contracts.Catalog.DomainEvents.CatalogItemUpdated;

namespace Application.UseCases.Events;

public class ProjectCatalogItemsWhenChangedConsumer :
    IConsumer<CatalogDeletedEvent>,
    IConsumer<CatalogItemAddedEvent>,
    IConsumer<CatalogItemRemovedEvent>,
    IConsumer<CatalogItemUpdatedEvent>
{
    private readonly ICatalogEventStoreService _eventStoreService;
    private readonly ICatalogProjectionsService _projectionsService;

    public ProjectCatalogItemsWhenChangedConsumer(
        ICatalogEventStoreService eventStoreService, 
        ICatalogProjectionsService projectionsService)
    {
        _eventStoreService = eventStoreService;
        _projectionsService = projectionsService;
    }

    public Task Consume(ConsumeContext<CatalogDeletedEvent> context)
        => ProjectAsync(context.Message.CatalogId, context.CancellationToken);

    public Task Consume(ConsumeContext<CatalogItemAddedEvent> context)
        => ProjectAsync(context.Message.CatalogId, context.CancellationToken);

    public Task Consume(ConsumeContext<CatalogItemRemovedEvent> context)
        => ProjectAsync(context.Message.CatalogId, context.CancellationToken);

    public Task Consume(ConsumeContext<CatalogItemUpdatedEvent> context)
        => ProjectAsync(context.Message.CatalogId, context.CancellationToken);

    private async Task ProjectAsync(Guid catalogId, CancellationToken cancellationToken)
    {
        var catalog = await _eventStoreService.LoadAggregateFromStreamAsync(catalogId, cancellationToken);

        var catalogItemsProjection = new CatalogItemsProjection
        {
            Id = catalog.Id,
            IsDeleted = catalog.IsDeleted,
            Items = catalog.Items
                .Select(item
                    => new CatalogItemProjection
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Name = item.Name,
                        Price = item.Price,
                        PictureUri = item.PictureUri
                    })
        };

        await _projectionsService.ProjectAsync(catalogItemsProjection, cancellationToken);
    }
}