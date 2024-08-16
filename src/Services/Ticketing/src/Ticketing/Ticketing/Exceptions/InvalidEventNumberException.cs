using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Ticketing.Ticketing.Exceptions;

public class InvalidEventNumberException : BadRequestException
{
    public InvalidEventNumberException(string eventNumber)
        : base($"Event Number: '{eventNumber}' is invalid.")
    {

    }
}
