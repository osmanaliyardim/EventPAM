using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Events.Models;
using EventPAM.Event.Repositories;
using MongoDB.Driver;

namespace EventPAM.Event.Events.Features.UpdatingEvent.V1;

public record UpdateEventMongo(Guid EventId, string EventNumber, Guid VenueId, decimal DurationMinutes, 
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted = false) 
        : InternalCommand;

internal class UpdateEventMongoCommandHandler : ICommandHandler<UpdateEventMongo>
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public UpdateEventMongoCommandHandler(
        IEventRepository eventRepository,
        IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateEventMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<EventReadModel>(request);

        var @event = await _eventRepository.FindOneAsync
            (e => e.EventId == eventReadModel.EventId && !e.IsDeleted, cancellationToken)
                ?? throw new EventNotFountException();

        await _eventRepository.UpdateAsync(
            Builders<EventReadModel>.Filter.Eq(e => e.EventId, eventReadModel.EventId),
            Builders<EventReadModel>.Update
                .Set(x => x.Price, eventReadModel.Price)
                .Set(x => x.VenueId, eventReadModel.VenueId)
                .Set(x => x.DurationMinutes, eventReadModel.DurationMinutes)
                .Set(x => x.EventDate, eventReadModel.EventDate)
                .Set(x => x.EventNumber, eventReadModel.EventNumber)
                .Set(x => x.Status, eventReadModel.Status)
                .Set(x => x.IsDeleted, eventReadModel.IsDeleted),
            cancellationToken);

        return Unit.Value;
    }
}
