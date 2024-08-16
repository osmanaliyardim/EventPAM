using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Venues.Exceptions;

public class InvalidCapacityException : BadRequestException
{
    public InvalidCapacityException() : base(Messages.INVALID_CAPACITY)
    {

    }
}
