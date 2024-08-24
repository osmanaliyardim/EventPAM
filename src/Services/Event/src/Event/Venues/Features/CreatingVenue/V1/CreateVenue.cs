using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Venues.Exceptions;
using EventPAM.Event.Venues.Models;
using EventPAM.Event.Venues.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using static EventPAM.Event.Events.Constants.Constants.Role;

namespace EventPAM.Event.Venues.Features.CreatingVenue.V1;

public record CreateVenue(string Name, int Capacity, Address Address) 
    : ICommand<CreateVenueResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateVenueResult(Guid VenueId);

public record VenueCreatedDomainEvent
    (Guid VenueId, string Name, int Capacity, Address Address, bool IsDeleted) : IDomainEvent;

public record CreateVenueRequestDto(string Name, int Capacity, Address Address);

public record CreateVenueResponseDto(Guid VenueId);

public class CreateVenueEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/event/venue", 
            async (CreateVenueRequestDto request, IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<CreateVenue>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<CreateVenueResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization(policy => policy.RequireRole([Admin, EventManager]))
            .WithName("CreateVenue")
            .WithApiVersionSet(builder.NewApiVersionSet("Event").Build())
            .Produces<CreateVenueResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Venue")
            .WithDescription("Create Venue")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateVenueValidator : AbstractValidator<CreateVenue>
{
    public CreateVenueValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Capacity).NotEmpty().WithMessage("Venue Capacity is required");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
    }
}

public class CreateVenueHandler : IRequestHandler<CreateVenue, CreateVenueResult>
{
    private readonly EventDbContext _eventDbContext;

    public CreateVenueHandler(EventDbContext eventDbContext)
    {
        _eventDbContext = eventDbContext;
    }

    public async Task<CreateVenueResult> Handle(CreateVenue request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var venue = await _eventDbContext.Venues.SingleOrDefaultAsync(
            v => v.Name.Value == request.Name, cancellationToken);

        if (venue is not null)
        {
            throw new VenueAlreadyExistException();
        }

        var entity = Venue.Create(VenueId.Of(request.Id), Name.Of(request.Name), Capacity.Of(request.Capacity), request.Address);

        var newVenue = (await _eventDbContext.Venues.AddAsync(entity, cancellationToken)).Entity;

        return new CreateVenueResult(newVenue.Id);
    }
}
