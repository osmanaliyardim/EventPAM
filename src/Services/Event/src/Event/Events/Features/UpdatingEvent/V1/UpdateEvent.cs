using EventPAM.BuildingBlocks.Behaviors.Caching;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Events.Features.CreatingEvent.V1;
using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Venues.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.Event.Events.Features.UpdatingEvent.V1;

public record UpdateEvent(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes,
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted) 
        : ICommand<UpdateEventResult>, IInternalCommand, ICacheRemoverRequest
{
    public bool BypassCache { get; }

    public string? CacheKey { get; }

    public string CacheGroupKey => "GetEvents";
}

public record UpdateEventResult(Guid Id);

public record EventUpdatedDomainEvent(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes,
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted) 
        : IDomainEvent;

public record UpdateEventRequestDto(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes, 
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted);

public class UpdateEventEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/event", 
            async (UpdateEventRequestDto request, IMediator mediator,
                IMapper mapper, CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<UpdateEvent>(request);

                var result = await mediator.Send(command, cancellationToken);

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("UpdateEvent")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Event")
            .WithDescription("Update Event")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class UpdateEventValidator : AbstractValidator<CreateEvent>
{
    public UpdateEventValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Status).Must(p => (p.GetType().IsEnum &&
                                          p == Enums.EventStatus.InAction) ||
                                         p == Enums.EventStatus.Cancelled ||
                                         p == Enums.EventStatus.Delay ||
                                         p == Enums.EventStatus.Completed)
            .WithMessage("Status must be InAction, Delay, Cancelled or Completed");

        RuleFor(x => x.VenueId).NotEmpty().WithMessage("VenueId cannot be empty");
        RuleFor(x => x.DurationMinutes).GreaterThan(0).WithMessage("DurationMinutes must be greater than 0");
        RuleFor(x => x.EventDate).NotEmpty().WithMessage("EventDate cannot be empty");
    }
}

internal class UpdateEventHandler : ICommandHandler<UpdateEvent, UpdateEventResult>
{
    private readonly EventDbContext _eventDbContext;

    public UpdateEventHandler(EventDbContext eventDbContext)
    {
        _eventDbContext = eventDbContext;
    }

    public async Task<UpdateEventResult> Handle(UpdateEvent request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var @event = await _eventDbContext.Events
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken) 
                ?? throw new EventNotFountException();

        @event.Update(EventId.Of(request.EventId), EventNumber.Of(request.EventNumber), VenueId.Of(request.VenueId),
             DurationMinutes.Of(request.DurationMinutes), EventDate.Of(request.EventDate), request.Status,
             Price.Of(request.Price), request.IsDeleted);

        var updatedEvent = _eventDbContext.Events.Update(@event).Entity;

        return new UpdateEventResult(updatedEvent.Id);
    }
}
