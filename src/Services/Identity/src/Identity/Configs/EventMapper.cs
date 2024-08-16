using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.Identity.Configs;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }
}
