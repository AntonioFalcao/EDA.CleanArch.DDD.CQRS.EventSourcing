using System.Diagnostics;
using Application.DependencyInjection;
using Infrastructure.EventStore.Contexts;
using Infrastructure.EventStore.DependencyInjection.Extensions;
using Infrastructure.EventStore.DependencyInjection.Options;
using Infrastructure.MessageBus.DependencyInjection.Extensions;
using Infrastructure.MessageBus.DependencyInjection.Options;
using Infrastructure.Projections.DependencyInjection.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

builder.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = true;
});

builder.ConfigureAppConfiguration(configurationBuilder =>
{
    configurationBuilder
        .AddUserSecrets<Program>()
        .AddEnvironmentVariables();
});

builder.ConfigureLogging((context, loggingBuilder) =>
{
    Log.Logger = new LoggerConfiguration().ReadFrom
        .Configuration(context.Configuration)
        .CreateLogger();

    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.ConfigureServices((context, services) =>
{
    services.AddEventStore();
    services.AddProjections();
    services.AddMessageBus();
    services.AddMessageValidators();
    services.AddNotificationContext();
    services.AddInteractors();

    services.ConfigureEventStoreOptions(
        context.Configuration.GetSection(nameof(EventStoreOptions)));

    services.ConfigureSqlServerRetryOptions(
        context.Configuration.GetSection(nameof(SqlServerRetryOptions)));

    services.ConfigureMessageBusOptions(
        context.Configuration.GetSection(nameof(MessageBusOptions)));

    services.ConfigureQuartzOptions(
        context.Configuration.GetSection(nameof(QuartzOptions)));

    services.ConfigureMassTransitHostOptions(
        context.Configuration.GetSection(nameof(MassTransitHostOptions)));
});

using var host = builder.Build();

var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

applicationLifetime.ApplicationStopping.Register(() =>
{
    Log.Information("Waiting 20s for a graceful termination...");
    Thread.Sleep(20000);
});

applicationLifetime.ApplicationStopped.Register(() =>
{
    Log.Information("Application completely stopped");
    Process.GetCurrentProcess().Kill();
});

try
{
    var environment = host.Services.GetRequiredService<IHostEnvironment>();

    if (environment.IsDevelopment() || environment.IsStaging())
    {
        await using var scope = host.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        await dbContext.Database.MigrateAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    await host.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await host.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    host.Dispose();
}