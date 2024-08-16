using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.Event.Seats.Models;

public record SeatReadModel : Aggregate<Guid>
{
    public required Guid SeatId { get; init; }

    public required string SeatNumber { get; init; }

    public required Enums.SeatClass Class { get; init; }

    public required Guid EventId { get; init; }
}
