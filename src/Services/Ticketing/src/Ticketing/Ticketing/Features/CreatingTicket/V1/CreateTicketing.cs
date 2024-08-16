using Ardalis.GuardClauses;
using Bogus.Extensions.Extras;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.BuildingBlocks.EventStoreDB.Repository;
using EventPAM.Ticketing.GrpcClient.Protos;
using EventPAM.Ticketing.Ticketing.Exceptions;
using EventPAM.Ticketing.Ticketing.ValueObjects;
using MapsterMapper;
using MassTransit;

namespace EventPAM.Ticketing.Ticketing.Features.CreatingTicket.V1;

public record CreateTicketing(Guid CustomerId, Guid EventId, string Description) : ICommand<CreateTicketingResult>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateTicketingResult(ulong Id);

public record TicketingCreatedDomainEvent(Guid Id, CustomerInfo CustomerInfo, EventDetails EventDetails) : Entity<Guid>, IDomainEvent;

public record CreateTicketingRequestDto(Guid CustomerId, Guid EventId, string Description);

public record CreateTicketingResponseDto(ulong Id);

public class CreateTicketingEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/ticketing", async (CreateTicketingRequestDto request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<CreateTicketing>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<CreateTicketingResponseDto>();

                return Results.Ok(response);
            })
            //.RequireAuthorization(nameof(ApiScope))
            .WithName("CreateTicketing")
            .WithApiVersionSet(builder.NewApiVersionSet("Ticketing").Build())
            .Produces<CreateTicketingResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Ticketing")
            .WithDescription("Create Ticketing")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateTicketingValidator : AbstractValidator<CreateTicketing>
{
    public CreateTicketingValidator()
    {
        RuleFor(x => x.EventId).NotNull().WithMessage("EventId is required!");
        RuleFor(x => x.CustomerId).NotNull().WithMessage("CustomerId is required!");
    }
}

internal class CreateTicketingCommandHandler : ICommandHandler<CreateTicketing, CreateTicketingResult>
{
    private readonly IEventStoreDBRepository<Models.Ticketing> _eventStoreDbRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly EventGrpcService.EventGrpcServiceClient _eventGrpcServiceClient;
    private readonly CustomerGrpcService.CustomerGrpcServiceClient _customerGrpcServiceClient;

    public CreateTicketingCommandHandler(IEventStoreDBRepository<Models.Ticketing> eventStoreDbRepository,
        ICurrentUserProvider currentUserProvider,
        IEventDispatcher eventDispatcher,
        EventGrpcService.EventGrpcServiceClient eventGrpcServiceClient,
        CustomerGrpcService.CustomerGrpcServiceClient customerGrpcServiceClient)
    {
        _eventStoreDbRepository = eventStoreDbRepository;
        _currentUserProvider = currentUserProvider;
        _eventDispatcher = eventDispatcher;
        _eventGrpcServiceClient = eventGrpcServiceClient;
        _customerGrpcServiceClient = customerGrpcServiceClient;
    }

    public async Task<CreateTicketingResult> Handle(CreateTicketing command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var @event =
            await _eventGrpcServiceClient.GetByIdAsync(
                new GetByIdRequest { Id = command.EventId.ToString() }, cancellationToken: cancellationToken)
                    ?? throw new EventNotFoundException();

        var customer =
            await _customerGrpcServiceClient.GetByIdAsync(
                new GetCustomerByIdRequest { Id = command.CustomerId.ToString() }, cancellationToken: cancellationToken);

        var emptySeat = (await _eventGrpcServiceClient
                .GetAvailableSeatsAsync(new GetAvailableSeatsRequest
                { EventId = command.EventId.ToString() },
                    cancellationToken: cancellationToken)
                .ResponseAsync)
            ?.SeatDtos?.FirstOrDefault();

        var reservation = await _eventStoreDbRepository.Find(command.Id, cancellationToken);

        if (reservation is not null && !reservation.IsDeleted)
        {
            throw new TicketingAlreadyExistException();
        }

        var aggregate = Models.Ticketing.Create(
            command.Id,
            customerInfo: CustomerInfo.Of(customer.CustomerDto?.Name!),
            eventDetails: EventDetails.Of(
                @event.EventDto.EventNumber, new Guid(@event.EventDto.VenueId),
                @event.EventDto.EventDate.ToDateTime(),
                (decimal)@event.EventDto.Price, command.Description,
                emptySeat?.SeatNumber!
            ),
            isDeleted: false,
            userId: _currentUserProvider.GetCurrentUserId()
        );

        // to ensure id passed to aggregate
        aggregate.Id = command.Id; 

        await _eventDispatcher.SendAsync(aggregate.DomainEvents, cancellationToken: cancellationToken);

        await _eventGrpcServiceClient.ReserveSeatAsync(new ReserveSeatRequest
        {
            EventId = @event.EventDto.EventId,
            SeatNumber = emptySeat?.SeatNumber
        }, cancellationToken: cancellationToken);

        var result = await _eventStoreDbRepository.Add(
            aggregate,
            cancellationToken);

        return new CreateTicketingResult(result);
    }
}
