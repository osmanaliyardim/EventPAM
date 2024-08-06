using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;

public class RabbitMQLogger : LoggerServiceBase
{
    public RabbitMQLogger(IConfiguration configuration)
    {
        RabbitMQConfiguration rabbitMQConfiguration = configuration
            .GetSection(Configs.SERILOG_RABBITMQ)
            .Get<RabbitMQConfiguration>()!;

        RabbitMQClientConfiguration config =
            new()
            {
                Port = rabbitMQConfiguration.Port,
                DeliveryMode = RabbitMQDeliveryMode.Durable,
                Exchange = rabbitMQConfiguration.Exchange!,
                Username = rabbitMQConfiguration.Username!,
                Password = rabbitMQConfiguration.Password!,
                ExchangeType = rabbitMQConfiguration.ExchangeType!,
                RouteKey = rabbitMQConfiguration.RouteKey!
            };
        rabbitMQConfiguration.Hostnames.ForEach(hostname => config.Hostnames.Add(hostname));

        Logger = new LoggerConfiguration().WriteTo
            .RabbitMQ(
                (clientConfiguration, sinkConfiguration) =>
                {
                    clientConfiguration.From(config);
                    sinkConfiguration.TextFormatter = new JsonFormatter();
                }
            )
            .CreateLogger();
    }
}
