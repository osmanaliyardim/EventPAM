namespace EventPAM.Ticketing.Ticketing.Dtos;

public record TicketingResponseDto(Guid Id, string Name, string EventNumber, Guid VenueId, 
    decimal Price, DateTime EventDate, string SeatNumber, string Description);
