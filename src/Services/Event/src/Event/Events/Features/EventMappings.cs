using EventPAM.Event.Events.Features.CreatingEvent.V1;
using EventPAM.Event.Events.Features.DeletingEvent.V1;
using EventPAM.Event.Events.Features.UpdatingEvent.V1;
using EventPAM.Event.Events.Models;
using MassTransit;

namespace EventPAM.Event.Events.Features;

public class EventMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Event, EventDto>()
            .ConstructUsing(x => new EventDto(x.Id, x.EventNumber, x.VenueId,
                            x.DurationMinutes, x.EventDate, x.Status, x.Price));

        config.NewConfig<CreateEventMongo, EventReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.EventId, s => s.EventId);

        config.NewConfig<Models.Event, EventReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.EventId, s => s.Id);

        config.NewConfig<EventReadModel, EventDto>()
            .Map(d => d.EventId, s => s.EventId);

        config.NewConfig<UpdateEventMongo, EventReadModel>()
            .Map(d => d.EventId, s => s.EventId);

        config.NewConfig<DeleteEventMongo, EventReadModel>()
            .Map(d => d.EventId, s => s.Id);

        config.NewConfig<CreateEventRequestDto, CreateEvent>()
            .ConstructUsing(x => new CreateEvent(x.EventNumber, x.VenueId,
                            x.DurationMinutes, x.EventDate, x.Status, x.Price));

        config.NewConfig<UpdateEventRequestDto, UpdateEvent>()
            .ConstructUsing(x => new UpdateEvent(x.EventId, x.EventNumber, x.VenueId,
                            x.DurationMinutes, x.EventDate, x.Status, x.Price, x.IsDeleted));
    }
}
