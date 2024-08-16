using EventPAM.Event.Events.Features.UpdatingEvent.V1;

namespace EventPAM.IntegrationTest.Event.Fakes;

public class FakeUpdateEventCommand : AutoFaker<UpdateEvent>
{
    public FakeUpdateEventCommand(EventPAM.Event.Events.Models.Event @event)
    {
        RuleFor(r => r.EventId, _ => @event.Id);
        RuleFor(r => r.VenueId, _ => @event.VenueId);
        RuleFor(r => r.EventNumber, r => "EV012");
        RuleFor(r => r.Price, _ => 1800);
        RuleFor(r => r.Status, _ => @event.Status);
    }
}
