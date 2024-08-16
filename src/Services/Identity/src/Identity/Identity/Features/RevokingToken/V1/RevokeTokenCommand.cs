using EventPAM.Identity.Identity.Services.AuthService;

namespace EventPAM.Identity.Identity.Features.RevokingToken.V1;

public record RevokeTokenCommand(string Token, string IPAddress) : IRequest<RevokedTokenResult>;

public record RevokedTokenResult(Guid Id, string Token);

public record RevokedTokenRequest(string Token, string IPAddress);

public record RevokedTokenResponse(Guid Id, string Token);

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
        var refreshToken = await _authService.GetRefreshTokenByToken(request.Token)
            ?? throw new RefreshTokenNotFoundException();

        if (refreshToken.Revoked is not null && DateTime.UtcNow >= refreshToken.Expires)
            throw new InvalidRefreshTokenException();

        await _authService.RevokeRefreshToken(refreshToken, request.IPAddress, reason: "Revoked without replacement");

        var revokedTokenResult = _mapper.Map<RevokedTokenResult>(refreshToken);

        return revokedTokenResult;
    }
}
