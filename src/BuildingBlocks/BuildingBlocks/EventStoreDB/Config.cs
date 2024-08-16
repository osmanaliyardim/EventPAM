using System.Reflection;
using EventPAM.BuildingBlocks.EventStoreDB.BackgroundWorkers;
using EventPAM.BuildingBlocks.EventStoreDB.Projections;
using EventPAM.BuildingBlocks.EventStoreDB.Repository;
using EventPAM.BuildingBlocks.EventStoreDB.Subscriptions;
using EventPAM.BuildingBlocks.Web;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

namespace EventPAM.BuildingBlocks.EventStoreDB;

public class EventStoreOptions
{
    public string ConnectionString { get; set; } = default!;
}

public record EventStoreDBOptions(
    bool UseInternalCheckpointing = true
);

public static class EventStoreDBConfigExtensions
{
    public static IServiceCollection AddEventStoreDB(this IServiceCollection services, IConfiguration config,
        EventStoreDBOptions? options = null)
    {
        services
            .AddSingleton(x=>
            {
                var eventStoreOptions = services.GetOptions<EventStoreOptions>(nameof(EventStoreOptions));
                return new EventStoreClient(EventStoreClientSettings.Create(eventStoreOptions.ConnectionString));
            })
            .AddScoped(typeof(IEventStoreDBRepository<>), typeof(EventStoreDBRepository<>))
            .AddTransient<EventStoreDBSubscriptionToAll, EventStoreDBSubscriptionToAll>();

        if (options?.UseInternalCheckpointing != false)
            services.AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();

        return services;
    }

    public static IServiceCollection AddEventStoreDBSubscriptionToAll(
        this IServiceCollection services,
        EventStoreDBSubscriptionToAllOptions? subscriptionOptions = null,
        bool checkpointToEventStoreDB = true)
    {
        if (checkpointToEventStoreDB)
            services.AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();

        return services.AddHostedService(serviceProvider =>
            {
                var logger =
                    serviceProvider.GetRequiredService<ILogger<BackgroundWorker>>();

                var eventStoreDBSubscriptionToAll =
                    serviceProvider.GetRequiredService<EventStoreDBSubscriptionToAll>();

                return new BackgroundWorker(
                    logger,
                    ct =>
                        eventStoreDBSubscriptionToAll.SubscribeToAll(
                            subscriptionOptions ?? new EventStoreDBSubscriptionToAllOptions(),
                            ct
                        )
                );
            }
        );
    }

    public static IServiceCollection AddProjections(this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        services.AddSingleton<IProjectionPublisher, ProjectionPublisher>();

        RegisterProjections(services, assembliesToScan!);

        return services;
    }

    private static void RegisterProjections(IServiceCollection services, Assembly[] assembliesToScan)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan)
            .AddClasses(classes => classes.AssignableTo<IProjectionProcessor>()) // Filter classes
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}
