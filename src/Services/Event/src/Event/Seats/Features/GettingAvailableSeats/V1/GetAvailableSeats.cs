using EventPAM.BuildingBlocks.Behaviors.Caching;
using EventPAM.BuildingBlocks.Core.Requests;
using EventPAM.BuildingBlocks.Core.Responses;
using EventPAM.Event.Repositories;
using EventPAM.Event.Seats.Dtos;
using EventPAM.Event.Seats.Exceptions;
using MongoDB.Driver;
using static EventPAM.Event.Events.Constants.Constants.Role;

namespace EventPAM.Event.Seats.Features.GettingAvailableSeats.V1;

public record GetAvailableSeats(Guid EventId, PageRequest? PageRequest) : IQuery<GetAvailableSeatsResult>, ICachableRequest
{
    public bool BypassCache { get; set; } = false;

    public string CacheKey => $"GetAvailableSeats({PageRequest!.PageIndex},{PageRequest.PageSize})";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);

    public string CacheGroupKey => "GetEvents";
}

public record GetAvailableSeatsResult(GetListResponse<SeatDto> SeatDtos);

public record GetAvailableSeatsResponseDto(GetListResponse<SeatDto> SeatDtos);

public class GetAvailableSeatsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/event/get-available-seats/{{eventId}}", GetAvailableSeats)
            .RequireAuthorization(policy => policy.RequireRole([Admin, Customer]))
            .WithName("GetAvailableSeats")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces<GetAvailableSeatsResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Available Seats")
            .WithDescription("Get Available Seats")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> GetAvailableSeats
        (Guid eventId, [AsParameters] PageRequest pageRequest,
            IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator
            .Send(new GetAvailableSeats(eventId, pageRequest), cancellationToken);

        var response = result.Adapt<GetAvailableSeatsResponseDto>();

        return Results.Ok(response);
    }
}

public class GetAvailableSeatsValidator : AbstractValidator<GetAvailableSeats>
{
    public GetAvailableSeatsValidator()
    {
        RuleFor(x => x.EventId).NotNull().WithMessage("EventId is required!");
    }
}

internal class GetAvailableSeatsQueryHandler : IRequestHandler<GetAvailableSeats, GetAvailableSeatsResult>
{
    private readonly IMapper _mapper;
    private readonly ISeatRepository _seatRepository;

    public GetAvailableSeatsQueryHandler(IMapper mapper, ISeatRepository seatRepository)
    {
        _mapper = mapper;
        _seatRepository = seatRepository;
    }

    public async Task<GetAvailableSeatsResult> Handle(GetAvailableSeats request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var seats = await _seatRepository.GetAllPaginatedAsync(
            predicate: s => s.EventId == request.EventId && !s.IsDeleted,
            orderBy: null,
            index: request.PageRequest!.PageIndex,
            size: request.PageRequest.PageSize,
            cancellationToken
        );

        if (!seats.Items.Any())
        {
            throw new AllSeatsFullException();
        }

        var seatDtos = _mapper.Map<GetListResponse<SeatDto>>(seats);

        return new GetAvailableSeatsResult(seatDtos);
    }
}
