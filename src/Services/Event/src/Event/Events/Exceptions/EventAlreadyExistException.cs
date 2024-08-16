using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Events.Exceptions;

public class EventAlreadyExistException : BadRequestException
{
    public EventAlreadyExistException() : base(Messages.EVENT_ALREADY_EXISTS)
    {

    }
}
