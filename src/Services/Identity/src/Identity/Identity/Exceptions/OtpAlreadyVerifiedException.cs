using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using static EventPAM.BuildingBlocks.EventPAMBase;

namespace EventPAM.Identity.Identity.Exceptions;

public class OtpAlreadyVerifiedException : BadRequestException
{
    public OtpAlreadyVerifiedException(string code = default!) : base(Messages.ALREADY_VERIFIED_OTP)
    {

    }
}
