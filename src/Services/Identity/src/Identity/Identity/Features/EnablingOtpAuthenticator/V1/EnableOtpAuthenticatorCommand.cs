using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Identity.Services.AuthenticatorService;
using EventPAM.Identity.Identity.Services.UserService;
using EventPAM.Identity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EventPAM.Identity.Identity.Features.EnablingOtpAuthenticator.V1;

public record EnableOtpAuthenticatorCommand(Guid UserId) : IRequest<EnableOtpAuthenticatorResult>;

public record EnableOtpAuthenticatorResult(string SecretKey);

public record EnabledOtpAuthenticatorResponse(string SecretKey);

public class EnableOtpAuthenticatorCommandValidator : AbstractValidator<EnableOtpAuthenticatorCommand>
{
    public EnableOtpAuthenticatorCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
                .NotNull()
                    .WithMessage("Please enter the UserId");
    }
}

public class EnableOtpAuthenticatorEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/identity/enable-otp-authenticator",
            async ( IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
            {
                var command = new EnableOtpAuthenticatorCommand(GetUserIdFromRequest());

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<EnabledOtpAuthenticatorResponse>();

                return Results.Ok(response);
            }
        )
        .WithName("EnableOtpAuthenticator")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces<EnabledOtpAuthenticatorResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Enable OTP Authenticator")
        .WithDescription("Enable OTP Authenticator")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class EnableOtpAuthenticatorCommandHandler : IRequestHandler<EnableOtpAuthenticatorCommand, EnableOtpAuthenticatorResult>
{
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;
    private readonly IUserService _userService;

    public EnableOtpAuthenticatorCommandHandler(
        IUserService userService,
        IOtpAuthenticatorRepository otpAuthenticatorRepository,
        IAuthenticatorService authenticatorService
    )
    {
        _userService = userService;
        _otpAuthenticatorRepository = otpAuthenticatorRepository;
        _authenticatorService = authenticatorService;
    }

    public async Task<EnableOtpAuthenticatorResult> Handle(
        EnableOtpAuthenticatorCommand request,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _userService.GetById(request.UserId);

        var isExistsOtpAuthenticator = await _otpAuthenticatorRepository
            .GetAsync(o => o.UserId == request.UserId, cancellationToken: cancellationToken);

        if (isExistsOtpAuthenticator is not null && isExistsOtpAuthenticator.IsVerified)
            throw new OtpAlreadyVerifiedException();

        if (isExistsOtpAuthenticator is not null)
            await _otpAuthenticatorRepository.DeleteAsync(isExistsOtpAuthenticator);

        var newOtpAuthenticator = 
            await _authenticatorService.CreateOtpAuthenticator(user);

        var addedOtpAuthenticator = 
            await _otpAuthenticatorRepository.AddAsync(newOtpAuthenticator);

        var secretKey = await _authenticatorService
            .ConvertSecretKeyToString(addedOtpAuthenticator.SecretKey);

        var enabledOtpAuthenticatorResult = 
            new EnableOtpAuthenticatorResult(secretKey);

        return enabledOtpAuthenticatorResult;
    }
}
