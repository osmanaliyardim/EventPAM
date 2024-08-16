using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Seats.Exceptions;

public class InvalidSeatNumberException : BadRequestException
{
    public InvalidSeatNumberException() : base(Messages.INVALID_SEAT_NUMBER)
    {

    }
}
