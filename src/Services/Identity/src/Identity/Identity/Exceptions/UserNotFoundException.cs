using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException() : base(Messages.USER_NOT_FOUND)
    {

    }
}
