using Application.Abstractions.EventStore;
using Domain.Aggregates;
using Domain.StoreEvents;

namespace Application.EventStore;

public interface ICatalogEventStoreRepository : IEventStoreRepository<Catalog, CatalogStoreEvent, CatalogSnapshot, Guid> { }