using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Events.Models;
using EventPAM.Event.Repositories;

namespace EventPAM.Event.Events.Features.CreatingEvent.V1;

public record CreateEventMongo(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes,
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted = false) 
        : InternalCommand;

internal class CreateEventMongoHandler : ICommandHandler<CreateEventMongo>
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public CreateEventMongoHandler(IEventRepository eventRepository,IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateEventMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<EventReadModel>(request);

        var @event = await _eventRepository.FindOneAsync
            (e => e.EventId == eventReadModel.EventId && !e.IsDeleted, cancellationToken);

        if (@event is not null)
        {
            throw new EventAlreadyExistException();
        }

        await _eventRepository.AddAsync(eventReadModel, cancellationToken);

        return Unit.Value;
    }
}
