namespace EventPAM.Event.Events.Exceptions;

public class EventNotFountException : BuildingBlocks.CrossCuttingConcerns.Exceptions.Types.NotFoundException
{
    public EventNotFountException() : base(Messages.EVENT_NOT_FOUND)
    {

    }
}
