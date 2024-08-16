using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Seats.Exceptions;

public class SeatAlreadyExistException : BadRequestException
{
    public SeatAlreadyExistException() : base(Messages.SEAT_ALREADY_EXISTS)
    {

    }
}
