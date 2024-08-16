namespace EventPAM.Event.Events.Exceptions;

using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

public class InvalidEventIdException : BadRequestException
{
    public InvalidEventIdException(Guid eventId)
        : base($"EventId: '{eventId}' is invalid.")
    {

    }
}
