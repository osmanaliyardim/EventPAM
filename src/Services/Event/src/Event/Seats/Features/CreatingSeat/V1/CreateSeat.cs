using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Seats.Exceptions;
using EventPAM.Event.Seats.Models;
using EventPAM.Event.Seats.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.Event.Events.Features.CreatingSeat.V1;

public record CreateSeat
    (string SeatNumber, Seats.Enums.SeatClass Class, Guid EventId) 
        : ICommand<CreateSeatResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateSeatResult(Guid SeatId);

public record SeatCreatedDomainEvent(Guid SeatId, string SeatNumber,
    Seats.Enums.SeatClass Class, Guid EventId, bool IsDeleted) 
        : IDomainEvent;

public record CreateSeatRequestDto(string SeatNumber, Seats.Enums.SeatClass Class, Guid EventId);

public record CreateSeatResponse(Guid SeatId);

public class CreateSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/event/seat", CreateSeat)
            .RequireAuthorization()
            .WithName("CreateSeat")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces<CreateSeatResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Seat")
            .WithDescription("Create Seat")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> CreateSeat(
        CreateSeatRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateSeat>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = result.Adapt<CreateSeatResponse>();

        return Results.Ok(response);
    }
}

public class CreateSeatValidator : AbstractValidator<CreateSeat>
{
    public CreateSeatValidator()
    {
        RuleFor(x => x.SeatNumber)
            .NotEmpty()
                .WithMessage("SeatNumber is required");

        RuleFor(x => x.EventId)
            .NotEmpty()
                .WithMessage("EventId is required");

        RuleFor(x => x.Class)
            .Must(p => (p.GetType().IsEnum 
                && p == Seats.Enums.SeatClass.Standard) 
                || p == Seats.Enums.SeatClass.Premium)
                .WithMessage("Status must be Standard or Premium");
    }
}

public class CreateSeatCommandHandler : IRequestHandler<CreateSeat, CreateSeatResult>
{
    private readonly EventDbContext _eventDbContext;

    public CreateSeatCommandHandler(EventDbContext eventDbContext)
    {
        _eventDbContext = eventDbContext;
    }

    public async Task<CreateSeatResult> Handle(CreateSeat command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _eventDbContext.Seats.SingleOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (seat is not null)
        {
            throw new SeatAlreadyExistException();
        }

        var seatEntity = Seat.Create(
            SeatId.Of(command.Id),
            SeatNumber.Of(command.SeatNumber),
            command.Class,
            EventId.Of(command.EventId),
            false
        );

        var newSeat = (await _eventDbContext.Seats.AddAsync(seatEntity, cancellationToken)).Entity;

        return new CreateSeatResult(newSeat.Id);
    }
}
