using Microsoft.AspNetCore.Routing;

namespace EventPAM.BuildingBlocks.Web;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
