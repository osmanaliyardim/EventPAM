using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Seats.Exceptions;

public class SeatNumberIncorrectException : BadRequestException
{
    public SeatNumberIncorrectException() : base(Messages.INCORRECT_SEAT_NUMBER)
    {

    }
}
