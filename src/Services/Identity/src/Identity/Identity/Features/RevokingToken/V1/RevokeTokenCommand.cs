using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Identity.Services.AuthService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace EventPAM.Identity.Identity.Features.RevokingToken.V1;

public record RevokeTokenCommand(string RefreshToken, string IPAddress) : IRequest<RevokedTokenResult>;

public record RevokedTokenResult(Guid Id, string Token);

public record RevokedTokenRequest(string RefreshToken, string IPAddress);

public record RevokedTokenResponse(Guid Id, string Token);

public class RevokeTokenEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/identity/revoke-token",
            async ([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RevokedTokenRequest request, 
            IMediator mediator, IHttpContextAccessor context, IMapper mapper, CancellationToken cancellationToken) =>
            {
                if (request is null ||request.RefreshToken is null)
                    request = new RevokedTokenRequest(GetRefreshTokenFromCookies(context)!, GetIpAddress(context)!);

                var command = mapper.Map<RevokeTokenCommand>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = mapper.Map<RevokedTokenResponse>(result);

                return Ok(result);
            }
        )
        .WithName("RevokeToken")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces<RevokedTokenResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Revoke Token")
        .WithDescription("Revoke Token")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, RevokedTokenResult>
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public RevokeTokenCommandHandler(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    public async Task<RevokedTokenResult> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _authService.GetRefreshTokenByToken(request.RefreshToken)
            ?? throw new RefreshTokenNotFoundException();

        if (refreshToken.Revoked is not null && DateTime.UtcNow >= refreshToken.Expires)
            throw new InvalidRefreshTokenException();

        await _authService.RevokeRefreshToken(refreshToken, request.IPAddress, reason: "Revoked without replacement");

        var revokedTokenResult = _mapper.Map<RevokedTokenResult>(refreshToken);

        return revokedTokenResult;
    }
}
