namespace EventPAM.BuildingBlocks.MassTransit;

public class RabbitMQOptions
{
    public string Host { get; set; } = "localhost";

    public string ExchangeName { get; set; } = default!;

    public string UserName { get; set; } = "guest";

    public string Password { get; set; } = "guest";

    public ushort? Port { get; set; } = 5672;
}
