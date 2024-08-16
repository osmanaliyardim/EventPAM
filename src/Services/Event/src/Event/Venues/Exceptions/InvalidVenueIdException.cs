using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Venues.Exceptions;

public class InvalidVenueIdException : BadRequestException
{
    public InvalidVenueIdException(Guid venueId)
        : base($"VenueId: '{venueId}' is invalid.")
    {

    }
}
