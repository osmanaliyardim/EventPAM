using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Seats.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.Event.Seats.Features.ReservingSeat.V1;

public record ReserveSeat(Guid EventId, string SeatNumber) 
    : ICommand<ReserveSeatResult>, IInternalCommand;

public record ReserveSeatResult(Guid SeatId);

public record SeatReservedDomainEvent(Guid SeatId, string SeatNumber, Enums.SeatClass Class,
    Guid EventId, bool IsDeleted) 
        : IDomainEvent;

public record ReserveSeatRequestDto(Guid EventId, string SeatNumber);

public record ReserveSeatResponseDto(Guid SeatId);

public class ReserveSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/event/reserve-seat", ReserveSeat)
            //.RequireAuthorization(nameof(ApiScope))
            .WithName("ReserveSeat")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces<ReserveSeatResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Reserve Seat")
            .WithDescription("Reserve Seat")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> ReserveSeat(ReserveSeatRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<ReserveSeat>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = result.Adapt<ReserveSeatResponseDto>();

        return Results.Ok(response);
    }
}

public class ReserveSeatValidator : AbstractValidator<ReserveSeat>
{
    public ReserveSeatValidator()
    {
        RuleFor(x => x.EventId).NotEmpty().WithMessage("EventId cannot be empty");
        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber cannot be empty");
    }
}

internal class ReserveSeatCommandHandler : IRequestHandler<ReserveSeat, ReserveSeatResult>
{
    private readonly EventDbContext _eventDbContext;

    public ReserveSeatCommandHandler(EventDbContext eventDbContext)
    {
        _eventDbContext = eventDbContext;
    }

    public async Task<ReserveSeatResult> Handle(ReserveSeat command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _eventDbContext.Seats
            .SingleOrDefaultAsync(s => s.SeatNumber.Value == command.SeatNumber 
                && s.EventId == command.EventId, cancellationToken) 
                    ?? throw new SeatNumberIncorrectException();

        seat.ReserveSeat();

        var updatedSeat = _eventDbContext.Seats.Update(seat).Entity;

        return new ReserveSeatResult(updatedSeat.Id);
    }
}
