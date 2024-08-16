using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class EmailAuthNotFoundException : NotFoundException
{
    public EmailAuthNotFoundException() : base(Messages.EMAIL_AUTH_NOT_FOUND)
    {

    }
}
