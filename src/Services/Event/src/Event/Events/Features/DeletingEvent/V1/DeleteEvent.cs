using EventPAM.BuildingBlocks.Core.Event;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.Event.Events.Features.DeletingEvent.V1;

public record DeleteEvent(Guid EventId) : ICommand<DeleteEventResult>, IInternalCommand;

public record DeleteEventResult(Guid EventId);

public record EventDeletedDomainEvent(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes,
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted) 
        : IDomainEvent;

public class DeleteEventEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/event/{{eventId}}",
                async (Guid eventId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(new DeleteEvent(eventId), cancellationToken);

                    return Results.NoContent();
                })
            //.RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteEvent")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Event")
            .WithDescription("Delete Event")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteEventValidator : AbstractValidator<DeleteEvent>
{
    public DeleteEventValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
    }
}

internal class DeleteEventHandler : ICommandHandler<DeleteEvent, DeleteEventResult>
{
    private readonly EventDbContext _eventDbContext;

    public DeleteEventHandler(EventDbContext eventDbContext)
    {
        _eventDbContext = eventDbContext;
    }

    public async Task<DeleteEventResult> Handle(DeleteEvent request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var @event = await _eventDbContext.Events.SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new EventNotFountException();

        @event.Delete(@event.Id, @event.EventNumber, @event.VenueId, @event.DurationMinutes,
            @event.EventDate, status: Enums.EventStatus.Cancelled, @event.Price, isDeleted: true);

        var deleteEvent = _eventDbContext.Events.Update(@event).Entity;

        return new DeleteEventResult(deleteEvent.Id);
    }
}
