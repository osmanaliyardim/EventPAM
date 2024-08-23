using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Enums;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Identity.Services.AuthenticatorService;
using EventPAM.Identity.Identity.Services.UserService;
using EventPAM.Identity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EventPAM.Identity.Identity.Features.VerifyingOtpAuthenticator.V1;

public record VerifyOtpAuthenticatorCommand(Guid UserId, string ActivationCode) : IRequest;

public record VerifyOtpAuthenticatorRequest(Guid? UserId, string AuthenticationCode);

public class VerifyOtpAuthenticatorEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/identity/verify-otp-authenticator",
            async ([FromBody] VerifyOtpAuthenticatorRequest request, IMediator mediator,
            IMapper mapper, IHttpContextAccessor context, CancellationToken cancellationToken) =>
            {
                var requestWithUserId = 
                    new VerifyOtpAuthenticatorRequest(GetUserIdFromRequest(context), request.AuthenticationCode);

                var command = mapper.Map<VerifyOtpAuthenticatorCommand>(requestWithUserId);

                await mediator.Send(command, cancellationToken);

                return Ok();
            }
        )
        .WithName("VerifyOTPAuthenticator")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Verify OTP Authenticator")
        .WithDescription("Verify OTP Authenticator")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

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
