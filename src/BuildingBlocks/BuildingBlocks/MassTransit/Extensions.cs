using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EventPAM.BuildingBlocks.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker
        (this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            if (assembly is not null)
                config.AddConsumers(assembly);

            // RabbitMQ Configuration
            config.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(new Uri(configuration[Configs.RABBITMQ_HOST]!), host =>
                {
                    host.Username(configuration[Configs.RABBITMQ_USERNAME]!);
                    host.Password(configuration[Configs.RABBITMQ_PASS]!);
                });
                configurator.ConfigureEndpoints(context);
            });

            // Azure Service Bus Configuration
            config.UsingAzureServiceBus((context, configurator) =>
            {
                configurator.Host(configuration[Configs.AZURE_SB_CONFIG]!);
                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
