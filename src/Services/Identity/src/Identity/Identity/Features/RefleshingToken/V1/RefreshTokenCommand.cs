using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Identity.Services.AuthService;
using EventPAM.Identity.Identity.Services.UserService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EventPAM.Identity.Identity.Features.RefreshingToken.V1;

public record RefreshTokenCommand(string? RefreshToken, string? IPAddress) : IRequest<RefreshedTokensResult>;

public record RefreshedTokensResult(AccessToken AccessToken, RefreshToken RefreshToken);

public class RefreshTokenEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/identity/refresh-token",
            async (IMediator mediator, IHttpContextAccessor context, 
            IMapper mapper, CancellationToken cancellationToken) =>
            {
                var command = new RefreshTokenCommand(GetRefreshTokenFromCookies(context), GetIpAddress(context));

                var result = await mediator.Send(command, cancellationToken);

                SetRefreshTokenToCookies(result.RefreshToken, context);

                return Results.Created(uri: "", result.AccessToken);
            }
        )
        .WithName("RefreshToken")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces<AccessToken>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Refresh JWT Token")
        .WithDescription("Refresh JWT Token")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshedTokensResult>
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public RefreshTokenCommandHandler(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    public async Task<RefreshedTokensResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var refreshToken = await _authService
            .GetRefreshTokenByToken(request.RefreshToken!);

        if (refreshToken!.Revoked is not null)
            await _authService.RevokeDescendantRefreshTokens(
                refreshToken,
                request.IPAddress!,
                reason: $"Attempted reuse of revoked ancestor token: {refreshToken.Token}"
            );
        
        if (refreshToken.Revoked is not null && DateTime.UtcNow >= refreshToken.Expires)
            throw new InvalidRefreshTokenException();

        var user = await _userService.GetById(refreshToken.UserId);

        var newRefreshToken = await _authService
            .RotateRefreshToken(user, refreshToken, request.IPAddress!);

        var addedRefreshToken = await _authService
            .AddRefreshToken(newRefreshToken);

        await _authService.DeleteOldRefreshTokens(refreshToken.UserId);

        var createdAccessToken = await _authService.CreateAccessToken(user);

        var refreshedTokensResult = new RefreshedTokensResult(createdAccessToken, addedRefreshToken);
        
        return refreshedTokensResult;
    }
}
