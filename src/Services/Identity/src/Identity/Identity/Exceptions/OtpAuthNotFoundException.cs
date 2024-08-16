using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class OtpAuthNotFoundException : NotFoundException
{
    public OtpAuthNotFoundException() : base(Messages.OTP_AUTH_NOT_FOUND)
    {

    }
}
