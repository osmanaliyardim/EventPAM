using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Venues.Exceptions;

public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base(Messages.INVALID_VENUE_NAME)
    {

    }
}
