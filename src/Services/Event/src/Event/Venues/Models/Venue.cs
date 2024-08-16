using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.Event.Venues.Features.CreatingVenue.V1;
using EventPAM.Event.Venues.ValueObjects;

namespace EventPAM.Event.Venues.Models;

public record Venue : Aggregate<VenueId>
{
    public Name Name { get; private set; } = default!;

    public Capacity Capacity { get; private set; } = default!;

    public static Venue Create(VenueId id, Name name, Capacity capacity, bool isDeleted = false)
    {
        var venue = new Venue
        {
            Id = id,
            Name = name,
            Capacity = capacity
        };

        var @event = new VenueCreatedDomainEvent(
            venue.Id,
            venue.Name,
            venue.Capacity,
            isDeleted);

        venue.AddDomainEvent(@event);

        return venue;
    }
}
