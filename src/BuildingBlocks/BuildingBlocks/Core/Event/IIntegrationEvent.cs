using MassTransit;

namespace EventPAM.BuildingBlocks.Core.Event;

[ExcludeFromTopology]
public interface IIntegrationEvent : IEvent
{

}
