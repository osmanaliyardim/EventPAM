using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() : base(Messages.INVALID_REFRESH_TOKEN)
    {

    }
}
