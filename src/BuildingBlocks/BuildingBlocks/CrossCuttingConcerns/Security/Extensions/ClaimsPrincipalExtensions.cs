using System.Security.Claims;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static List<string>? Claims(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        List<string>? result = claimsPrincipal?.FindAll(claimType)?.Select(x => x.Value).ToList();
        return result;
    }

    public static List<string>? ClaimRoles(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal?.Claims(ClaimTypes.Role);

    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal) =>
        new(claimsPrincipal?.Claims(ClaimTypes.NameIdentifier)?.FirstOrDefault()!);
}
