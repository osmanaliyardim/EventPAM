using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Events.Models;
using EventPAM.Event.Repositories;
using MongoDB.Driver;

namespace EventPAM.Event.Events.Features.DeletingEvent.V1;

public record DeleteEventMongo(Guid Id, string EventNumber, Guid VenueId, decimal DurationMinutes, 
    DateTime EventDate, Enums.EventStatus Status, decimal Price, bool IsDeleted = true) 
        : InternalCommand;

internal class DeleteEventMongoCommandHandler : ICommandHandler<DeleteEventMongo>
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public DeleteEventMongoCommandHandler(IEventRepository eventRepository, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteEventMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<EventReadModel>(request);

        var @event = await _eventRepository.FindOneAsync
            (e => e.EventId == eventReadModel.EventId && !e.IsDeleted, cancellationToken)
                ?? throw new EventNotFountException();

        // Soft delete (setting IsDeleted as true - still accessable)
        await _eventRepository.UpdateAsync(
            Builders<EventReadModel>.Filter.Eq(e => e.EventId, eventReadModel.EventId),
            Builders<EventReadModel>.Update
                .Set(x => x.IsDeleted, eventReadModel.IsDeleted),
            cancellationToken);

        // Hard delete (directly removing from DB - no way to access again)
        //await _eventRepository.DeleteAsync(
        //    e => e.EventId == eventReadModel.EventId,
        //    cancellationToken);

        return Unit.Value;
    }
}
