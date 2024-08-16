namespace EventPAM.Event.Seats.Dtos;

public record SeatDto
    (Guid SeatId, string SeatNumber, Enums.SeatClass Class, Guid EventId);
