using EventPAM.Event.Data.Seed;
using EventPAM.Event.Events.Enums;
using EventPAM.Event.Events.Features.CreatingEvent.V1;
using MassTransit;

namespace EventPAM.EndToEndTest.Event.Fakes;

public sealed class FakeCreateEventMongoCommand : AutoFaker<CreateEventMongo>
{
    public FakeCreateEventMongoCommand()
    {
        RuleFor(r => r.EventId, _ => NewId.NextGuid());
        RuleFor(r => r.EventNumber, r => "EV011");
        RuleFor(r => r.Status, _ => EventStatus.InAction);
        RuleFor(r => r.VenueId, _ => InitialData.Venues.First().Id);
        RuleFor(r => r.IsDeleted, _ => false);
    }
}
