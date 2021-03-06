using Application.UseCases.Events.Integrations;
using Application.UseCases.Events.Projections;
using Contracts.Abstractions.Messages;
using Contracts.Services.ShoppingCart;
using MassTransit;
using DomainEvent = Contracts.Services.Order.DomainEvent;

namespace Infrastructure.MessageBus.DependencyInjection.Extensions;

internal static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void ConfigureEventReceiveEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IRegistrationContext context)
    {
        cfg.ConfigureEventReceiveEndpoint<ProjectOrderDetailsWhenOrderChangedConsumer, DomainEvent.OrderPlaced>(context);
        cfg.ConfigureEventReceiveEndpoint<ProjectOrderDetailsWhenOrderChangedConsumer, DomainEvent.OrderConfirmed>(context);
        cfg.ConfigureEventReceiveEndpoint<PlaceOrderWhenCartSubmittedConsumer, IntegrationEvent.CartSubmitted>(context);
    }

    private static void ConfigureEventReceiveEndpoint<TConsumer, TEvent>(this IRabbitMqBusFactoryConfigurator bus, IRegistrationContext context)
        where TConsumer : class, IConsumer
        where TEvent : class, IEvent
        => bus.ReceiveEndpoint(
            queueName: $"order.{typeof(TConsumer).ToKebabCaseString()}.{typeof(TEvent).ToKebabCaseString()}",
            configureEndpoint: endpoint =>
            {
                endpoint.ConfigureConsumeTopology = false;
                endpoint.Bind<TEvent>();
                endpoint.ConfigureConsumer<TConsumer>(context);
            });
}