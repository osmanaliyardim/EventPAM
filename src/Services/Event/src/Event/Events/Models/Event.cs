using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.Event.Events.Features.CreatingEvent.V1;
using EventPAM.Event.Events.Features.DeletingEvent.V1;
using EventPAM.Event.Events.Features.UpdatingEvent.V1;
using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Venues.ValueObjects;

namespace EventPAM.Event.Events.Models;

public record Event : Aggregate<EventId>
{
    public EventNumber EventNumber { get; private set; } = default!;

    public VenueId VenueId { get; private set; } = default!;

    public DurationMinutes DurationMinutes { get; private set; } = default!;

    public Enums.EventStatus Status { get; private set; }

    public Price Price { get; private set; } = default!;

    public EventDate EventDate { get; private set; } = default!;

    public static Event Create(EventId id, EventNumber eventNumber, VenueId venueId,
        DurationMinutes durationMinutes, EventDate eventDate, Enums.EventStatus status,
        Price price, bool isDeleted = false)
    {
        var @event = new Event
        {
            Id = id,
            EventNumber = eventNumber,
            VenueId = venueId,
            DurationMinutes = durationMinutes,
            EventDate = eventDate,
            Status = status,
            Price = price,
            IsDeleted = isDeleted
        };

        var eventing = new EventCreatedDomainEvent(@event.Id, @event.EventNumber, @event.VenueId,
            @event.DurationMinutes, @event.EventDate, @event.Status,
            @event.Price, @event.IsDeleted);

        @event.AddDomainEvent(eventing);

        return @event;
    }

    public void Update(EventId id, EventNumber eventNumber, VenueId venueId,
        DurationMinutes durationMinutes, EventDate eventDate, Enums.EventStatus status,
        Price price, bool isDeleted = false)
    {
        EventNumber = eventNumber;
        VenueId = venueId;
        DurationMinutes = durationMinutes;
        EventDate = eventDate;
        Status = status;
        Price = price;
        IsDeleted = isDeleted;

        var @event = new EventUpdatedDomainEvent(id, eventNumber, venueId, durationMinutes,
            eventDate, status, price, isDeleted);

        AddDomainEvent(@event);
    }

    public void Delete(EventId id, EventNumber eventNumber, VenueId venueId,
        DurationMinutes durationMinutes, EventDate eventDate, Enums.EventStatus status,
        Price price, bool isDeleted = true)
    {
        EventNumber = eventNumber;
        VenueId = venueId;
        DurationMinutes = durationMinutes;
        EventDate = eventDate;
        Status = status;
        Price = price;
        IsDeleted = isDeleted;

        var @event = new EventDeletedDomainEvent(id, eventNumber, venueId, durationMinutes,
            eventDate, status, price, isDeleted);

        AddDomainEvent(@event);
    }
}
