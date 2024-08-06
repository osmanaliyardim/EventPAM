using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
}
