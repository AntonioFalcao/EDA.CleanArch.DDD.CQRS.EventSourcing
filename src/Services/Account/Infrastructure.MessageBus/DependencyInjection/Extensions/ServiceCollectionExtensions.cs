using System.Reflection;
using Application.Abstractions.Notifications;
using Contracts.Abstractions.Messages;
using Contracts.JsonConverters;
using FluentValidation;
using Infrastructure.MessageBus.DependencyInjection.Options;
using Infrastructure.MessageBus.Notifications;
using Infrastructure.MessageBus.PipeFilters;
using Infrastructure.MessageBus.PipeObservers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Infrastructure.MessageBus.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
        => services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                cfg.AddConsumers(Assembly.Load(nameof(Application)));
                cfg.AddConsumers(Assembly.GetExecutingAssembly());

                cfg.UsingRabbitMq((context, bus) =>
                {
                    var options = context.GetRequiredService<IOptionsMonitor<MessageBusOptions>>().CurrentValue;

                    bus.Host(options.ConnectionString);

                    cfg.AddMessageScheduler(new Uri($"queue:{options.SchedulerQueueName}"));

                    bus.UseInMemoryScheduler(
                        schedulerFactory: context.GetRequiredService<ISchedulerFactory>(), 
                        queueName: options.SchedulerQueueName);

                    bus.UseMessageRetry(retry
                        => retry.Incremental(
                            retryLimit: options.RetryLimit,
                            initialInterval: options.InitialInterval,
                            intervalIncrement: options.IntervalIncrement));

                    bus.UseNewtonsoftJsonSerializer();

                    bus.ConfigureNewtonsoftJsonSerializer(settings =>
                    {
                        settings.Converters.Add(new TypeNameHandlingConverter(TypeNameHandling.Objects));
                        settings.Converters.Add(new DateOnlyJsonConverter());
                        settings.Converters.Add(new ExpirationDateOnlyJsonConverter());
                        return settings;
                    });

                    bus.ConfigureNewtonsoftJsonDeserializer(settings =>
                    {
                        settings.Converters.Add(new TypeNameHandlingConverter(TypeNameHandling.Objects));
                        settings.Converters.Add(new DateOnlyJsonConverter());
                        settings.Converters.Add(new ExpirationDateOnlyJsonConverter());
                        return settings;
                    });

                    bus.MessageTopology.SetEntityNameFormatter(new KebabCaseEntityNameFormatter());
                    bus.UseConsumeFilter(typeof(ContractValidatorFilter<>), context);
                    bus.UseConsumeFilter(typeof(BusinessValidatorFilter<>), context);
                    bus.ConnectReceiveObserver(new LoggingReceiveObserver());
                    bus.ConnectConsumeObserver(new LoggingConsumeObserver());
                    bus.ConnectPublishObserver(new LoggingPublishObserver());
                    bus.ConnectSendObserver(new LoggingSendObserver());
                    bus.ConfigureEventReceiveEndpoints(context);
                    bus.ConfigureEndpoints(context);
                });
            })
            .AddQuartz();

    public static IServiceCollection AddMessageValidators(this IServiceCollection services)
        => services.AddValidatorsFromAssemblyContaining(typeof(IMessage));

    public static IServiceCollection AddNotificationContext(this IServiceCollection services)
        => services.AddScoped<INotificationContext, NotificationContext>();

    public static OptionsBuilder<MessageBusOptions> ConfigureMessageBusOptions(this IServiceCollection services, IConfigurationSection section)
        => services
            .AddOptions<MessageBusOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

    public static OptionsBuilder<QuartzOptions> ConfigureQuartzOptions(this IServiceCollection services, IConfigurationSection section)
        => services
            .AddOptions<QuartzOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

    public static OptionsBuilder<MassTransitHostOptions> ConfigureMassTransitHostOptions(this IServiceCollection services, IConfigurationSection section)
        => services
            .AddOptions<MassTransitHostOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}