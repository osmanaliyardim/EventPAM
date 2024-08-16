using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Venues.Exceptions;

public class VenueAlreadyExistException : BadRequestException
{
    public VenueAlreadyExistException() : base(Messages.VENUE_ALREADY_EXISTS)
    {

    }
}
