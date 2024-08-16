using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Ticketing.Ticketing.Exceptions;

public class SeatNumberException : BadRequestException
{
    public SeatNumberException(string seatNumber)
        : base($"Seat Number: '{seatNumber}' is invalid.")
    {

    }
}

