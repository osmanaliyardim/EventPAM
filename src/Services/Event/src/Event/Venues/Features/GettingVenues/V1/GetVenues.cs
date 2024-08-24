using EventPAM.BuildingBlocks.Behaviors.Caching;
using EventPAM.BuildingBlocks.Core.Requests;
using EventPAM.BuildingBlocks.Core.Responses;
using EventPAM.Event.Repositories;
using EventPAM.Event.Venues.Dtos;
using EventPAM.Event.Venues.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static EventPAM.Event.Events.Constants.Constants.Role;

namespace EventPAM.Event.Venues.Features.GettingVenues.V1;

public record GetVenues(PageRequest? PageRequest) : IQuery<GetVenuesResult>, ICachableRequest
{
    public bool BypassCache => false;

    public string CacheKey => $"GetVenues({PageRequest!.PageIndex},{PageRequest.PageSize})";

    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(1);

    public string CacheGroupKey => "GetVenues";
}

public record GetVenuesResult(GetListResponse<VenueDto> VenueDtos);

public record GetVenuesResponseDto(GetListResponse<VenueDto> VenueDtos);

public class GetVenuesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/venues", GetVenues)
            .RequireAuthorization(policy => policy.RequireRole([Admin, EventManager, Customer]))
            .WithName("GetVenues")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces<GetVenuesResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Venues")
            .WithDescription("Get Venues")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> GetVenues
        ([AsParameters] PageRequest pageRequest, IMapper mapper,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var paginatedEvents = new GetVenues(pageRequest);

        var result = await mediator.Send(paginatedEvents, cancellationToken);

        var response = result.Adapt<GetVenuesResponseDto>();

        return Results.Ok(response);
    }
}

internal class GetVenuesHandler : IQueryHandler<GetVenues, GetVenuesResult>
{
    private readonly IMapper _mapper;
    private readonly IVenueRepository _venueRepository;

    public GetVenuesHandler(IMapper mapper, IVenueRepository venueRepository)
    {
        _mapper = mapper;
        _venueRepository = venueRepository;
    }

    public async Task<GetVenuesResult> Handle(GetVenues request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var venues = await _venueRepository.GetAllPaginatedAsync(
            predicate: v => !v.IsDeleted,
            orderBy: q => q.OrderBy(v => v.Name).ThenBy(v => v.CreatedAt),
            index: request.PageRequest!.PageIndex,
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken
        );

        if (!venues.Items.Any())
            throw new VenueNotFoundException();

        var venueDtos = _mapper.Map<GetListResponse<VenueDto>>(venues);

        return new GetVenuesResult(venueDtos);
    }
}

