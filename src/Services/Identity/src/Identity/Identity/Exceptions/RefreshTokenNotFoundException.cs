using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class RefreshTokenNotFoundException : NotFoundException
{
    public RefreshTokenNotFoundException() : base(Messages.REFRESH_TOKEN_NOT_FOUND)
    {

    }
}
