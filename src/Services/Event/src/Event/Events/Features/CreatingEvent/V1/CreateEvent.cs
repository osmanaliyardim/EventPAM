using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Venues.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.Event.Events.Features.CreatingEvent.V1;

public record CreateEvent(string EventNumber, Guid VenueId, decimal DurationMinutes, 
    DateTime EventDate, Enums.EventStatus Status, decimal Price) 
        : ICommand<CreateEventResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateEventResult(Guid EventId);

public record EventCreatedDomainEvent(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes,
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted) 
        : IDomainEvent;

public record CreateEventRequestDto(string EventNumber, Guid VenueId, decimal DurationMinutes, 
    DateTime EventDate, Enums.EventStatus Status, decimal Price);

public record CreateEventResponseDto(Guid EventId);

public class CreateEventEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/event", 
            async (CreateEventRequestDto request, IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<CreateEvent>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<CreateEventResponseDto>();

                return Results.CreatedAtRoute("GetEventById", new { id = result.EventId }, response);
            })
            //.RequireAuthorization(nameof(ApiScope))
            .WithName("CreateEvent")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces<CreateEventResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Event")
            .WithDescription("Create Event")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateEventValidator : AbstractValidator<CreateEvent>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.Status)
            .Must(p => (p.GetType().IsEnum &&
                                         p == Enums.EventStatus.InAction) ||
                                         p == Enums.EventStatus.Canceled ||
                                         p == Enums.EventStatus.Delay ||
                                         p == Enums.EventStatus.Completed)
            .WithMessage("Status must be InAction, Delay, Canceled or Completed");

        RuleFor(x => x.VenueId)
            .NotEmpty()
            .WithMessage("VenueId must be not empty");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("DurationMinutes must be greater than 0");
        
        RuleFor(x => x.EventDate)
            .NotEmpty()
            .WithMessage("EventDate must be not empty");
    }
}

public class CreateEventHandler : ICommandHandler<CreateEvent, CreateEventResult>
{
    private readonly EventDbContext _eventDbContext;

    public CreateEventHandler(EventDbContext eventDbContext)
    {
        _eventDbContext = eventDbContext;
    }

    public async Task<CreateEventResult> Handle(CreateEvent request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var @event = await _eventDbContext.Events.SingleOrDefaultAsync(x => x.Id == request.Id,
            cancellationToken);

        if (@event is not null)
        {
            throw new EventAlreadyExistException();
        }

        var eventEntity = Models.Event.Create(EventId.Of(request.Id), EventNumber.Of(request.EventNumber),
            VenueId.Of(request.VenueId), DurationMinutes.Of(request.DurationMinutes), EventDate.Of(request.EventDate),
            request.Status, Price.Of(request.Price));

        var newEvent = (await _eventDbContext.Events.AddAsync(eventEntity, cancellationToken)).Entity;

        return new CreateEventResult(newEvent.Id);
    }
}
