using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Core.CQRS;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Dtos;
using EventPAM.Identity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.Identity.Identity.Features.GettingUserById.V1;

public record GetUserByIdQuery(Guid Id) : IQuery<GetUserByIdResult>;

public record GetUserByIdResult(UserDto UserDto);

public record GetUsertByIdResponse(UserDto UserDto);

public class GetUsertByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUsertByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

public class GeUserByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/identity/get-user-by-id/{{id}}",
            async (Guid id, IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);

                var response = result.Adapt<GetUsertByIdResponse>();

                return Results.Ok(response);
            })
        //.RequireAuthorization(nameof(ApiScope))
        .WithName("GetUserByIdQuery")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces<GetUsertByIdResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get User By Id")
        .WithDescription("Get User By Id")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(
        IMapper mapper, 
        IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<GetUserByIdResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _userRepository.GetAsync(
            predicate: u => u.Id == request.Id,
            include: u => u.Include(u => u.UserOperationClaims).ThenInclude(uoc => uoc.OperationClaim),
            enableTracking: false,
            cancellationToken: cancellationToken
        ) ?? throw new UserNotFoundException();

        var userDto = _mapper.Map<UserDto>(user);

        return new GetUserByIdResult(userDto);
    }
}
