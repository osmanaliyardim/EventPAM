using EventPAM.Event.Venues.Exceptions;

namespace EventPAM.Event.Venues.ValueObjects;

public record VenueId
{
    public Guid Value { get; }

    private VenueId(Guid value)
    {
        Value = value;
    }

    public static VenueId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidVenueIdException(value);
        }

        return new VenueId(value);
    }

    public static implicit operator Guid(VenueId venueId)
    {
        return venueId.Value;
    }
}
