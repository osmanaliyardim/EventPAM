using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Events.Features.CreatingEvent.V1;
using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Events.Features.DeletingEvent.V1;
using EventPAM.Event.Events.Features.UpdatingEvent.V1;
using EventPAM.Event.Seats.Features.ReservingSeat.V1;
using EventPAM.Event.Venues.Features.CreatingVenue.V1;

namespace EventPAM.Event;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            EventCreatedDomainEvent e => new EventCreated(e.EventId),
            EventUpdatedDomainEvent e => new EventUpdated(e.EventId),
            EventDeletedDomainEvent e => new EventDeleted(e.EventId),
            VenueCreatedDomainEvent e => new VenueCreated(e.VenueId),
            SeatCreatedDomainEvent e => new SeatCreated(e.SeatId),
            SeatReservedDomainEvent e => new SeatReserved(e.SeatId),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            EventCreatedDomainEvent e => new CreateEventMongo(e.EventId, e.EventNumber, e.VenueId, e.DurationMinutes, 
                e.EventDate, e.Status, e.Price, e.IsDeleted),
            EventUpdatedDomainEvent e => new UpdateEventMongo(e.EventId, e.EventNumber, e.VenueId, e.DurationMinutes,
                e.EventDate, e.Status, e.Price, e.IsDeleted),
            EventDeletedDomainEvent e => new DeleteEventMongo(e.EventId, e.EventNumber, e.VenueId, e.DurationMinutes,
                e.EventDate, e.Status, e.Price, e.IsDeleted),
            VenueCreatedDomainEvent e => new CreateVenueMongo(e.VenueId, e.Name, e.Capacity, e.IsDeleted),
            SeatCreatedDomainEvent e => new CreateSeatMongo(e.SeatId, e.SeatNumber, e.Class, e.EventId, e.IsDeleted),
            SeatReservedDomainEvent e => new ReserveSeatMongo(e.SeatId, e.SeatNumber, e.Class, e.EventId, e.IsDeleted),
            _ => null
        };
    }
}
