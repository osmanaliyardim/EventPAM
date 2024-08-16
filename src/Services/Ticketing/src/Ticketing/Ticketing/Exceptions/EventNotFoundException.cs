using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Ticketing.Ticketing.Exceptions;

public class EventNotFoundException : NotFoundException
{
    public EventNotFoundException() : base(Messages.EVENT_NOT_FOUND)
    {

    }
}
