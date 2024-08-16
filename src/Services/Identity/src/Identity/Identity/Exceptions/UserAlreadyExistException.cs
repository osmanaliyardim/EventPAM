using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class UserAlreadyExistException : BadRequestException
{
    public UserAlreadyExistException(string code = default!) : base(Messages.USER_ALREADY_EXISTS)
    {

    }
}
