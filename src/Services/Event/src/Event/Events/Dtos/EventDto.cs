namespace EventPAM.Event.Events.Dtos;

public record EventDto(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes, 
    DateTime EventDate, Enums.EventStatus Status, decimal Price);
