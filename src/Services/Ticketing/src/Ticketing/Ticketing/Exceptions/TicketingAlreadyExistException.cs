using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Ticketing.Ticketing.Exceptions;

public class TicketingAlreadyExistException : BadRequestException
{
    public TicketingAlreadyExistException() : base(Messages.TICKET_ALREADY_EXIST)
    {

    }
}
