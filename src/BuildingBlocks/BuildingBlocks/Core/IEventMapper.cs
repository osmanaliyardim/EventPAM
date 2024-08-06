using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Core;

public interface IEventMapper
{
    IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event);

    IInternalCommand? MapToInternalCommand(IDomainEvent @event);
}
