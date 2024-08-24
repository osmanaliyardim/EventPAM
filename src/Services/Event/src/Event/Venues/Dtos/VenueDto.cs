using EventPAM.Event.Venues.Models;

namespace EventPAM.Event.Venues.Dtos;

public record VenueDto(Guid VenueId, string Name, int Capacity, Address Address);
