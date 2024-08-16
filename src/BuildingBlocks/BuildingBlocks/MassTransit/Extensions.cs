using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using EventPAM.BuildingBlocks.Web;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.BuildingBlocks.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker
        (this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env, Assembly? assembly = null)
    {
        services.AddValidateOptions<RabbitMQOptions>();

        if (env.IsEnvironment("tests"))
        {
            services.AddMassTransitTestHarness(configure =>
            {
                SetupMasstransitConfigurations(services, configure, assembly);
            });
        }
        else
        {
            services.AddMassTransit(config =>
            {
                services.AddMassTransit(configure => { SetupMasstransitConfigurations(services, configure, assembly); });
            });
        }

        return services;
    }

    private static void SetupMasstransitConfigurations(IServiceCollection services,
    IBusRegistrationConfigurator configure, Assembly? assembly)
    {
        configure.SetKebabCaseEndpointNameFormatter();

        if (assembly is not null)
            configure.AddConsumers(assembly);

        configure.AddSagaStateMachines(assembly);
        configure.AddSagas(assembly);
        configure.AddActivities(assembly);

        //configure.AddConfigureEndpointsCallback((_, cfg) =>
        //{
        //    if (cfg is IServiceBusReceiveEndpointConfigurator sb)
        //    {
        //        sb.ConfigureDeadLetterQueueDeadLetterTransport();
        //        sb.ConfigureDeadLetterQueueErrorTransport();
        //    }
        //});  

        configure.UsingRabbitMq((context, configurator) =>
        {
            var rabbitMqOptions = services.GetOptions<RabbitMQOptions>(nameof(RabbitMQOptions));

            configurator.Host(rabbitMqOptions?.Host, rabbitMqOptions?.Port ?? 5672, "/", hc =>
            {
                hc.Username(rabbitMqOptions?.UserName!);
                hc.Password(rabbitMqOptions?.Password!);
            });

            configurator.ConfigureEndpoints(context);

            configurator.UseMessageRetry(AddRetryConfiguration);
        });

        //configure.UsingAzureServiceBus((context, config) =>
        //{
        //    var azureServiceBusOptions = services.GetOptions<ASBOptions>(nameof(ASBOptions));

        //    config.Host(new Uri(azureServiceBusOptions.ConnectionString));

        //    config.ConfigureEndpoints(context);
        //});
    }

    private static void AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator.Exponential(
                3,
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMinutes(120),
                TimeSpan.FromMilliseconds(200))
            .Ignore<ValidationException>(); // don't retry if we have invalid data and message goes to _error queue masstransit
    }
}
