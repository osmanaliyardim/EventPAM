using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Ticketing.Ticketing.Features.CreatingTicket.V1;

namespace EventPAM.Ticketing;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            TicketingCreatedDomainEvent e => new TicketingCreated(e.Id),
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
