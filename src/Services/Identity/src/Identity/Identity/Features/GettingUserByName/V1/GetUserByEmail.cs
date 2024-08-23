using Ardalis.GuardClauses;
using Duende.IdentityServer.EntityFramework.Entities;
using EventPAM.BuildingBlocks.Core.CQRS;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Dtos;
using EventPAM.Identity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.Identity.Identity.Features.GettingUserByName.V1;

public record GetUserByEmailQuery(string Email) : IQuery<GetUserByEmailResult>;

public record GetUserByEmailResult(UserDto UserDto);

public record GetUserByEmailResponse(UserDto UserDto);

public class GeUserByNameEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/identity/get-user/{{email}}",
            async (string email, IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetUserByEmailQuery(email), cancellationToken);

                var response = result.Adapt<GetUserByEmailResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetUserByEmailQuery")
            .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
            .Produces<GetUserByEmailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User By Email")
            .WithDescription("Get User By Email")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GetUsertByEmailValidator : AbstractValidator<GetUserByEmailQuery>
{
    public GetUsertByEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .NotNull()
                    .WithMessage("Email is required!");
    }
}

internal class GetUserByNameQueryHandler : IQueryHandler<GetUserByEmailQuery, GetUserByEmailResult>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public GetUserByNameQueryHandler(
        IMapper mapper,
        IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<GetUserByEmailResult> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _userRepository.GetAsync(
            predicate: u => u.Email == request.Email,
            include: u => u.Include(u => u.UserOperationClaims).ThenInclude(uoc => uoc.OperationClaim),
            enableTracking: false,
            cancellationToken: cancellationToken
        ) ?? throw new UserNotFoundException();

        var userDto = _mapper.Map<UserDto>(user);

        return new GetUserByEmailResult(userDto);
    }
}

