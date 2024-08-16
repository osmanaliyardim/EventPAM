using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class InvalidPasswordException : BadRequestException
{
    public InvalidPasswordException() : base(Messages.INVALID_PASSWORD)
    {

    }
}
