using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.Event.Events.Models;

public record EventReadModel : Aggregate<Guid>
{
    public required Guid EventId { get; init; }

    public required string EventNumber { get; init; }

    public required Guid VenueId { get; init; }

    public required decimal DurationMinutes { get; init; }

    public required DateTime EventDate { get; init; }

    public required Enums.EventStatus Status { get; init; }

    public required decimal Price { get; init; }
}
