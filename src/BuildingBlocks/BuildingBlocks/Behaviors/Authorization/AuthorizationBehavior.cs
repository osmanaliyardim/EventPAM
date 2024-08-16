using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Constants;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace EventPAM.BuildingBlocks.Behaviors.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<string>? userRoleClaims = _httpContextAccessor.HttpContext!.User.ClaimRoles();

        if (userRoleClaims == null)
            throw new AuthorizationException(Messages.NOT_AUTHENTICATED);

        bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoleClaims
            .FirstOrDefault(
                userRoleClaim => userRoleClaim == GeneralOperationClaims.Admin || request.Roles.Any(role => role == userRoleClaim)
            )
            .IsNullOrEmpty();
        if (isNotMatchedAUserRoleClaimWithRequestRoles)
            throw new AuthorizationException(Messages.NOT_AUTHORIZED);

        TResponse response = await next();
        return response;
    }
}
