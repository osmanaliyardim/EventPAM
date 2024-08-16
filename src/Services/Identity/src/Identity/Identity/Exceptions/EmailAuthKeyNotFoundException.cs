using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class EmailAuthKeyNotFoundException : NotFoundException
{
    public EmailAuthKeyNotFoundException() : base(Messages.EMAIL_AUTHKEY_NOT_FOUND)
    {

    }
}
