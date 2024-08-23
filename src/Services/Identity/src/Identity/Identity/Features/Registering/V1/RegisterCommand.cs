using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.Core.CQRS;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Constants;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Enums;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Hashing;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Identity.Services.AuthService;
using EventPAM.Identity.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace EventPAM.Identity.Identity.Features.Registering.V1;

public record RegisterCommand(string FirstName, string LastName, string Email,
    string Password, string ConfirmPassword, string? IPAddress)
        : ICommand<RegisteredResult>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record RegisteredResult(string FirstName, string LastName, string Email,
    AccessToken AccessToken, RefreshToken RefreshToken);

public record RegisteredRequest(string FirstName, string LastName, string Email,
    string Password, string ConfirmPassword, string? IPAddress);

public record RegisteredResponse(string FirstName, string LastName, string Email,
    string AccessToken, string RefreshToken);

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
                .MinimumLength(2)
                    .WithMessage("FirstName is not valid or empty!");

        RuleFor(c => c.LastName)
            .NotEmpty()
                .MinimumLength(2)
                    .WithMessage("LastName is not valid or empty!");

        RuleFor(c => c.Email)
            .NotEmpty()
                .EmailAddress()
                    .WithMessage("Email Address is not valid or empty!");

        RuleFor(c => c.Password)
            .NotEmpty()
                .MinimumLength(4)
                    .WithMessage("Password is not valid or empty!");

        RuleFor(c => c.ConfirmPassword)
            .NotEmpty()
                .MinimumLength(4)
                    .WithMessage("ConfirmPassword is not valid or empty!");

        RuleFor(c => c).Custom((c, context) =>
        {
            if (c.Password != c.ConfirmPassword)
            {
                context.AddFailure(nameof(c.Password), "Passwords should match");
            }
        });

    }
}

public class RegisterUserEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/identity/register-user",
            async (RegisteredRequest request, IMediator mediator, IMapper mapper,
                IHttpContextAccessor context, CancellationToken cancellationToken) =>
            {
                var requestWithIp = new RegisteredRequest(request.FirstName, request.LastName,
                    request.Email, request.Password, request.ConfirmPassword, GetIpAddress(context));

                var command = mapper.Map<RegisterCommand>(requestWithIp);

                var result = await mediator.Send(command, cancellationToken);

                SetRefreshTokenToCookies(result.RefreshToken);

                var response = result.Adapt<RegisteredResponse>();

                return Created(uri: "", response);
            }
        )
        .WithName("RegisterUser")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces<RegisteredResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Register User")
        .WithDescription("Register User")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisteredResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IAuthService _authService;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IUserOperationClaimRepository userOperationClaimRepository,
        IOperationClaimRepository operationClaimRepository,
        IAuthService authService,
        IEventDispatcher eventDispatcher,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _userOperationClaimRepository = userOperationClaimRepository;
        _operationClaimRepository = operationClaimRepository;
        _authService = authService;
        _eventDispatcher = eventDispatcher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RegisteredResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _userRepository.GetAsync(
            u => u.Email == request.Email,
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (user is not null)
            throw new UserAlreadyExistException();

        HashingHelper.CreatePasswordHash(
            request.Password,
            out byte[] passwordHash,
            out byte[] passwordSalt);

        var newUser = new User(
            request.Id,
            request.FirstName,
            request.LastName,
            request.Email,
            passwordSalt,
            passwordHash,
            AuthenticatorType.None,
            false);

        var createdUser = await _userRepository.AddAsync(newUser);

        var customerOperationClaim = new UserOperationClaim
        {
            UserId = createdUser.Id,
            OperationClaimId = (await _operationClaimRepository.GetAsync
            (
                oc => oc.Name == GeneralOperationClaims.Customer, 
                cancellationToken: cancellationToken
            ))!.Id
        };
        await _userOperationClaimRepository.AddAsync(customerOperationClaim);

        var createdAccessToken = await _authService.CreateAccessToken(createdUser);
        var createdRefreshToken = await _authService.CreateRefreshToken(createdUser, request.IPAddress!);
        await _authService.AddRefreshToken(createdRefreshToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customerOperationClaim.UserId.ToString()),
            new(ClaimTypes.Name, customerOperationClaim.User.FirstName + " " + customerOperationClaim.User.LastName),
            new(ClaimTypes.Role, customerOperationClaim.OperationClaim.Name),
            new(ClaimTypes.Email, customerOperationClaim.User.Email)
        };

        var identity = new ClaimsIdentity(claims, "CustomScheme");
        _httpContextAccessor.HttpContext!.User = new ClaimsPrincipal(identity);

        await _eventDispatcher.SendAsync(
            new UserCreated(
                Id: request.Id,
                Name: request.FirstName + " " + request.LastName
            ),
            cancellationToken: cancellationToken
        );

        return new RegisteredResult(createdUser.FirstName, createdUser.LastName, createdUser.Email, 
            createdAccessToken, createdRefreshToken);
    }
}
