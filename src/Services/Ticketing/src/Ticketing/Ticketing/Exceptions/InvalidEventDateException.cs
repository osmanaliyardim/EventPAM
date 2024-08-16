using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Ticketing.Ticketing.Exceptions;

public class InvalidEventDateException : BadRequestException
{
    public InvalidEventDateException(DateTime eventDate)
        : base($"Event Date: '{eventDate}' is invalid.")
    {

    }
}
