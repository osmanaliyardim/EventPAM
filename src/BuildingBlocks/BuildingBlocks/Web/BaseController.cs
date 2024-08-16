using Asp.Versioning;
using Duende.IdentityServer.Models;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace EventPAM.BuildingBlocks.Web;

[Route(Configs.BASE_API_PATH)]
[ApiController]
[ApiVersion("1.0")]
public abstract class BaseController : ControllerBase
{
    private IMapper _mapper = default!;
    private IMediator _mediator = default!;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;

    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>()!;

    protected string? GetIpAddress(IHttpContextAccessor context)
    {
        if (context.HttpContext!.Request.Headers.ContainsKey("X-Forwarded-For"))
            return context.HttpContext.Request.Headers["X-Forwarded-For"];

        return context.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    protected int GetUserIdFromRequest()
    {
        int userId = HttpContext.User.GetUserId();
        
        return userId;
    }
}
