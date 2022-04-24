﻿using Application.EventStore;
using Domain.Aggregates;
using ECommerce.Contracts.Warehouses;
using MassTransit;

namespace Application.UseCases.Commands;

public class ReceiveInventoryItemConsumer : IConsumer<Command.ReceiveInventoryItem>
{
    private readonly IWarehouseEventStoreService _eventStore;

    public ReceiveInventoryItemConsumer(IWarehouseEventStoreService eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Consume(ConsumeContext<Command.ReceiveInventoryItem> context)
    {
        var inventoryItem = new InventoryItem();
        inventoryItem.Handle(context.Message);
        await _eventStore.AppendEventsAsync(inventoryItem, context.CancellationToken);
    }
}