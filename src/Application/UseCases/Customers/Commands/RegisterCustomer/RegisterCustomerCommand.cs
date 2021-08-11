﻿using System.Threading.Tasks;
using Application.Abstractions.UseCases;
using Application.EventSourcing.Customers.EventStore;
using Domain.Entities.Customers;
using MassTransit;

namespace Application.UseCases.Customers.Commands.RegisterCustomer
{
    public record RegisterCustomerCommand(string Name, int Age) : ICommand;
    
    public class RegisterCustomerCommandHandler : IConsumer<RegisterCustomerCommand>
    {
        private readonly ICustomerEventStoreService _eventStoreService;

        public RegisterCustomerCommandHandler(ICustomerEventStoreService eventStoreService)
        {
            _eventStoreService = eventStoreService;
        }

        public async Task Consume(ConsumeContext<RegisterCustomerCommand> context)
        {
            var customer = new Customer();
            var (name, age) = context.Message;
            customer.Register(name, age);
            await _eventStoreService.AppendEventsToStreamAsync(customer, context.CancellationToken);
        }
    }
}