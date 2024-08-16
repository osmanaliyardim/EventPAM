using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Events.Exceptions;

public class InvalidDurationException : BadRequestException
{
    public InvalidDurationException() : base(Messages.INVALID_DURATION)
    {

    }
}
