using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Enums;
using EventPAM.Identity.Identity.Services.AuthenticatorService;
using EventPAM.Identity.Identity.Services.UserService;
using EventPAM.Identity.Repositories;

namespace EventPAM.Identity.Identity.Features.VerifyingOtpAuthenticator.V1;

public record VerifyOtpAuthenticatorCommand(Guid UserId, string ActivationCode) : IRequest;

internal class VerifyOtpAuthenticatorCommandHandler : IRequestHandler<VerifyOtpAuthenticatorCommand>
{
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;
    private readonly IUserService _userService;

    public VerifyOtpAuthenticatorCommandHandler(
        IOtpAuthenticatorRepository otpAuthenticatorRepository,
        IUserService userService,
        IAuthenticatorService authenticatorService
    )
    {
        _otpAuthenticatorRepository = otpAuthenticatorRepository;
        _userService = userService;
        _authenticatorService = authenticatorService;
    }

    public async Task Handle(VerifyOtpAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var otpAuthenticator = await _otpAuthenticatorRepository.GetAsync(e => e.UserId == request.UserId)
            ?? throw new OtpAuthNotFoundException();

        var user = await _userService.GetById(request.UserId);

        otpAuthenticator.IsVerified = true;
        user.AuthenticatorType = AuthenticatorType.Otp;

        await _authenticatorService.VerifyAuthenticatorCode(user, request.ActivationCode);

        await _otpAuthenticatorRepository.UpdateAsync(otpAuthenticator);
        await _userService.Update(user);
    }
}
