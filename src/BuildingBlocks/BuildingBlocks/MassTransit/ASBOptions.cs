namespace EventPAM.BuildingBlocks.MassTransit;

public class ASBOptions
{
    public string ConnectionString { get; set; } = "sb://your-service-bus-namespace.servicebus.windows.net";
}
