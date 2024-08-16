using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Seats.Exceptions;

public class InvalidSeatIdException : BadRequestException
{
    public InvalidSeatIdException(Guid seatId)
        : base($"SeatId: '{seatId}' is invalid.")
    {

    }
}
