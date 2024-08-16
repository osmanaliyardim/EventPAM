using EventPAM.Event.Data.Seed;
using EventPAM.Event.Events.Enums;
using EventPAM.Event.Events.Features.CreatingEvent.V1;
using MassTransit;

namespace EventPAM.UnitTest.Event.Fakes;

public sealed class FakeCreateEventCommand : AutoFaker<CreateEvent>
{
    public FakeCreateEventCommand()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.EventNumber, r => "EV012");
        RuleFor(r => r.Status, _ => EventStatus.InAction);
        RuleFor(r => r.VenueId, _ => InitialData.Venues.First().Id);
    }
}
