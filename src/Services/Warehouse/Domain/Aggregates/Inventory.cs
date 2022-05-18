﻿using Domain.Abstractions.Aggregates;
using Contracts.Abstractions;
using Contracts.Services.Warehouse;
using Domain.Entities.Adjustments;
using Domain.Entities.InventoryItems;
using Domain.ValueObjects.Products;
using MassTransit;
using MassTransit.Internals;

namespace Domain.Aggregates;

public class Inventory : AggregateRoot<Guid, InventoryValidator>
{
    private readonly List<InventoryItem> _items = new();

    public Guid OwnerId { get; private set; }

    public int TotalItems
        => Items.Count();

    public int TotalUnits
        => Items.Sum(item => item.Quantity);

    public IEnumerable<InventoryItem> Items
        => _items;

    public void Handle(Command.CreateInventory cmd)
        => RaiseEvent(new DomainEvent.InventoryCreated(Guid.NewGuid(), cmd.OwnerId));

    public void Handle(Command.ReceiveInventoryItem cmd)
    {
        var raiseEvent = new Action(() => {});

        var item = _items.FirstOrDefault(inventoryItem => inventoryItem.Product == cmd.Product);

        if (item is null or { IsDeleted: true })
        {
            raiseEvent += () => new DomainEvent.InventoryItemReceived(cmd.InventoryId, Guid.NewGuid(), cmd.Product, cmd.Cost, cmd.Markup, cmd.Quantity);
        }
        else if (item is { IsDeleted: false })
        {
            raiseEvent += () => new DomainEvent.InventoryItemIncreased(cmd.InventoryId, item.Id, cmd.Quantity);

            if (item.Cost != cmd.Cost)
            {
                // InventoryItemCostRecalculated 
            }

            if (item.Markup != cmd.Markup)
            {
                // InventoryItemMarkupRecalculated
            }
        }

        raiseEvent();
    }

    public void Handle(Command.DecreaseInventoryAdjust cmd)
    {
        if (_items.FirstOrDefault(inventoryItem => inventoryItem.Id == cmd.InventoryItemId) is {IsDeleted: false} item)
            RaiseEvent(item.QuantityAvailable >= cmd.Quantity
                ? new DomainEvent.InventoryAdjustmentDecreased(cmd.InventoryId, cmd.InventoryItemId, cmd.Reason, cmd.Quantity)
                : new DomainEvent.InventoryAdjustmentNotDecreased(cmd.InventoryId, cmd.InventoryItemId, cmd.Reason, cmd.Quantity, item.QuantityAvailable));
    }

    public void Handle(Command.IncreaseInventoryAdjust cmd)
    {
        if (_items.FirstOrDefault(item => item.Id == cmd.InventoryItemId) is {IsDeleted: false})
            RaiseEvent(new DomainEvent.InventoryAdjustmentIncreased(cmd.InventoryId, cmd.InventoryItemId, cmd.Reason, cmd.Quantity));
    }

    public void Handle(Command.ReserveInventoryItem cmd)
    {
        if (_items.FirstOrDefault(inventoryItem => inventoryItem.Product == cmd.Product) is {IsDeleted: false} item)
            RaiseEvent(item.QuantityAvailable switch
            {
                < 1 => new DomainEvent.StockDepleted(item.Id),
                var availability when availability >= cmd.Quantity => new DomainEvent.InventoryReserved(Id, cmd.CartId, item.Id, cmd.Quantity),
                _ => new DomainEvent.InventoryNotReserved(Id, cmd.CartId, item.Id, cmd.Quantity, item.QuantityAvailable),
            });
    }

    protected override void ApplyEvent(IEvent @event)
        => When(@event as dynamic);

    private void When(DomainEvent.InventoryCreated @event)
        => (Id, OwnerId) = @event;

    private void When(DomainEvent.InventoryItemReceived @event)
        => _items.Add(new(@event.InventoryItemId, @event.Cost, @event.Markup, @event.Product, @event.Quantity));

    private void When(DomainEvent.InventoryAdjustmentDecreased @event)
        => _items
            .First(item => item.Id == @event.InventoryItemId)
            .Adjust(new DecreaseAdjustment(@event.Reason, @event.Quantity));

    private void When(DomainEvent.InventoryAdjustmentIncreased @event)
        => _items
            .First(item => item.Id == @event.InventoryItemId)
            .Adjust(new IncreaseAdjustment(@event.Reason, @event.Quantity));

    // private void When(DomainEvent.StockDepleted _)
    // {
    //     Status = OutOfStock;
    // }

    private void When(DomainEvent.InventoryReserved @event)
        => _items
            .First(item => item.Id == @event.InventoryItemId)
            .Reserve(@event.Quantity, @event.CartId);

    private void When(DomainEvent.InventoryNotReserved _) { }
}