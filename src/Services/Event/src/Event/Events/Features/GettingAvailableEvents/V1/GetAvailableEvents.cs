using EventPAM.BuildingBlocks.Behaviors.Caching;
using EventPAM.BuildingBlocks.Core.Requests;
using EventPAM.BuildingBlocks.Core.Responses;
using EventPAM.Event.Repositories;
using MongoDB.Driver.Linq;

namespace EventPAM.Event.Events.Features.GettingAvailableEvents.V1;

public record GetAvailableEvents : IQuery<GetAvailableEventsResult>, ICachableRequest
{
    public PageRequest PageRequest { get; set; } = default!;

    public bool BypassCache => false;

    public string CacheKey => $"GetAvailableEvents({PageRequest.PageIndex},{PageRequest.PageSize})";

    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(1);

    public string CacheGroupKey => "GetEvents";
}

public record GetAvailableEventsResult(GetListResponse<EventDto> EventDtos);

public record GetAvailableEventsResponseDto(GetListResponse<EventDto> EventDtos);

public class GetAvailableEventsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/event/get-available-events",
                async ([AsParameters] PageRequest pageRequest, IMediator mediator, 
                        CancellationToken cancellationToken) =>
                {
                    var paginatedEvents = new GetAvailableEvents()
                    {
                        PageRequest = pageRequest
                    };

                    var result = await mediator.Send(paginatedEvents, cancellationToken);

                    var response = result.Adapt<GetAvailableEventsResponseDto>();

                    return Results.Ok(response);
                })
                .WithName("GetAvailableEvents")
                .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
                .Produces<GetAvailableEventsResponseDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Available Events")
                .WithDescription("Get Available Events")
                .WithOpenApi()
                .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetAvailableEventsHandler : IQueryHandler<GetAvailableEvents, GetAvailableEventsResult>
{
    private readonly IMapper _mapper;
    private readonly IEventRepository _eventRepository;

    public GetAvailableEventsHandler(
        IMapper mapper, IEventRepository eventRepository)
    {
        _mapper = mapper;
        _eventRepository = eventRepository;
    }

    public async Task<GetAvailableEventsResult> Handle(GetAvailableEvents request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var events = await _eventRepository.GetAllPaginatedAsync(
            predicate: e => !e.IsDeleted,
            orderBy: q => q.OrderBy(e => e.EventDate).ThenBy(e => e.Price),
            index: request.PageRequest.PageIndex,
            size: request.PageRequest.PageSize,
            cancellationToken
        );

        if (!events.Items.Any())
        {
            throw new EventNotFountException();
        }

        var eventDtos = _mapper.Map<GetListResponse<EventDto>>(events);

        return new GetAvailableEventsResult(eventDtos);
    }
}
