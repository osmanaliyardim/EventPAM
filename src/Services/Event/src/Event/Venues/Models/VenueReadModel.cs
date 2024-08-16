using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.Event.Venues.Models;

public record VenueReadModel : Aggregate<Guid>
{
    public required Guid VenueId { get; init; }

    public required string Name { get; init; }

    public required int Capacity { get; init; }
}
