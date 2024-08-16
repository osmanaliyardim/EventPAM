using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EventPAM.Event.Events.Features.GettingEventById.V1;

public record GetEventById(Guid Id) : IQuery<GetEventByIdResult>;

public record GetEventByIdResult(EventDto EventDto);

public record GetEventByIdResponseDto(EventDto EventDto);

public class GetEventByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/event/{{id}}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetEventById(id), cancellationToken);

                    var response = result.Adapt<GetEventByIdResponseDto>();

                    return Results.Ok(response);
                })
            //.RequireAuthorization(nameof(ApiScope))
            .WithName("GetEventById")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces<GetEventByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Event By Id")
            .WithDescription("Get Event By Id")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GetEventByIdValidator : AbstractValidator<GetEventById>
{
    public GetEventByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

internal class GetEventByIdHandler : IQueryHandler<GetEventById, GetEventByIdResult>
{
    private readonly IMapper _mapper;
    private readonly EventReadDbContext _EventReadDbContext;

    public GetEventByIdHandler(IMapper mapper, EventReadDbContext EventReadDbContext)
    {
        _mapper = mapper;
        _EventReadDbContext = EventReadDbContext;
    }

    public async Task<GetEventByIdResult> Handle(GetEventById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var @event =
            await _EventReadDbContext.Events.AsQueryable().SingleOrDefaultAsync(x => x.EventId == request.Id &&
                !x.IsDeleted, cancellationToken)
                    ?? throw new EventNotFountException();

        var eventDto = _mapper.Map<EventDto>(@event);

        return new GetEventByIdResult(eventDto);
    }
}
