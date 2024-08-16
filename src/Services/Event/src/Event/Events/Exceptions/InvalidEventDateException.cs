using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Events.Exceptions;

public class InvalidEventDateException : BadRequestException
{
    public InvalidEventDateException(DateTime eventDate)
        : base($"Event Date: '{eventDate}' is invalid.")
    {

    }
}
