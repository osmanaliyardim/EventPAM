using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Seats.Features.ReservingSeat.V1;
using EventPAM.Event.Seats.ValueObjects;

namespace EventPAM.Event.Seats.Models;

public record Seat : Aggregate<SeatId>
{
    public SeatNumber SeatNumber { get; private set; } = default!;

    public Enums.SeatClass Class { get; private set; }

    public EventId EventId { get; private set; } = default!;

    public static Seat Create(SeatId id, SeatNumber seatNumber, Enums.SeatClass @class, 
        EventId eventId, bool isDeleted = false)
    {
        var seat = new Seat()
        {
            Id = id,
            Class = @class,
            SeatNumber = seatNumber,
            EventId = eventId,
            IsDeleted = isDeleted
        };

        var @event = new SeatCreatedDomainEvent(
            seat.Id,
            seat.SeatNumber,
            seat.Class,
            seat.EventId,
            seat.IsDeleted);

        seat.AddDomainEvent(@event);

        return seat;
    }

    public void ReserveSeat()
    {
        IsDeleted = true;
        LastModified = DateTime.Now;

        var @event = new SeatReservedDomainEvent(
            Id,
            SeatNumber,
            Class,
            EventId,
            IsDeleted);

        AddDomainEvent(@event);
    }
}
