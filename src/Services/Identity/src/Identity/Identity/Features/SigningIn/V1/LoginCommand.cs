using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Dtos;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Enums;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Hashing;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Identity.Services.AuthenticatorService;
using EventPAM.Identity.Identity.Services.AuthService;
using EventPAM.Identity.Identity.Services.UserService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EventPAM.Identity.Identity.Features.SigningIn.V1;

public record LoginCommand(UserForLoginDto UserForLoginDto, string? IPAddress) : IRequest<LoggedResult>;

public record LoggedResult
{
    public AccessToken? AccessToken { get; set; }

    public RefreshToken? RefreshToken { get; set; }

    public AuthenticatorType? RequiredAuthenticatorType { get; set; }

    public LoggedHttpResponse ToHttpResponse()
    {
        return new() { AccessToken = AccessToken, RequiredAuthenticatorType = RequiredAuthenticatorType };
    }

    public class LoggedHttpResponse
    {
        public AccessToken? AccessToken { get; set; }

        public AuthenticatorType? RequiredAuthenticatorType { get; set; }
    }
}

public record LoginRequest(UserForLoginDto UserForLoginDto, string? IPAddress);

public record LoggedResponse
{
    public AccessToken? AccessToken { get; set; }

    public RefreshToken? RefreshToken { get; set; }

    public AuthenticatorType? RequiredAuthenticatorType { get; set; }

    public LoggedHttpResponse ToHttpResponse()
    {
        return new LoggedHttpResponse() { AccessToken = AccessToken, RequiredAuthenticatorType = RequiredAuthenticatorType };
    }

    public record LoggedHttpResponse
    {
        public AccessToken? AccessToken { get; set; }

        public AuthenticatorType? RequiredAuthenticatorType { get; set; }
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.UserForLoginDto.Email)
            .NotEmpty()
                .EmailAddress()
                    .WithMessage("Email Address is invalid or empty!");

        RuleFor(c => c.UserForLoginDto.Password)
            .NotEmpty()
                .MinimumLength(4)
                    .WithMessage("Password is invalid or empty!");
    }
}

public class LoginEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/identity/login-user",
            async (LoginRequest request, IMediator mediator, IMapper mapper,
                IHttpContextAccessor context, CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<LoginCommand>(request);

                var result = await mediator.Send(command, cancellationToken);

                if (result.RefreshToken is not null)
                    SetRefreshTokenToCookies(result.RefreshToken, context);

                var response = result.Adapt<LoggedResponse>();

                return Results.Ok(response.ToHttpResponse());
            }
        )
        .WithName("LoginUser")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces<LoggedResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Login User")
        .WithDescription("Login User")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class LoginCommandHandler : IRequestHandler<LoginCommand, LoggedResult>
{
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public LoginCommandHandler(
        IUserService userService,
        IAuthService authService,
        IAuthenticatorService authenticatorService
    )
    {
        _userService = userService;
        _authService = authService;
        _authenticatorService = authenticatorService;
    }

    public async Task<LoggedResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByEmail(request.UserForLoginDto.Email)
            ?? throw new UserNotFoundException();

        var isValidPassword = HashingHelper.VerifyPasswordHash
            (request.UserForLoginDto.Password, user.PasswordHash, user.PasswordSalt);

        if (!isValidPassword)
            throw new InvalidPasswordException();

        var loggedResult = new LoggedResult();

        if (user.AuthenticatorType is not AuthenticatorType.None)
        {
            if (request.UserForLoginDto.AuthenticatorCode is null)
            {
                await _authenticatorService.SendAuthenticatorCode(user);
                loggedResult.RequiredAuthenticatorType = user.AuthenticatorType;
                
                return loggedResult;
            }

            await _authenticatorService.VerifyAuthenticatorCode(user, request.UserForLoginDto.AuthenticatorCode);
        }

        var createdAccessToken = await _authService.CreateAccessToken(user);

        var createdRefreshToken = await _authService.CreateRefreshToken(user, request.IPAddress!);
        var addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);

        await _authService.DeleteOldRefreshTokens(user.Id);

        loggedResult.AccessToken = createdAccessToken;
        loggedResult.RefreshToken = addedRefreshToken;

        return loggedResult;
    }
}