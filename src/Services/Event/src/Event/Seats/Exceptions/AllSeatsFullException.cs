using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Seats.Exceptions;

public class AllSeatsFullException : BadRequestException
{
    public AllSeatsFullException() : base(Messages.ALL_SEATS_FULL)
    {

    }
}
