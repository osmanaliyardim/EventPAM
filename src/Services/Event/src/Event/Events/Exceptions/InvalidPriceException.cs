using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Event.Events.Exceptions;

public class InvalidPriceException : BadRequestException
{
    public InvalidPriceException() : base(Messages.INVALID_PRICE)
    {

    }
}
