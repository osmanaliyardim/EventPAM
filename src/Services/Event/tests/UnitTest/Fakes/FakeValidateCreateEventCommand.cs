using EventPAM.Event.Events.Enums;
using EventPAM.Event.Events.Features.CreatingEvent.V1;

namespace EventPAM.UnitTest.Event.Fakes;

public class FakeValidateCreateEventCommand : AutoFaker<CreateEvent>
{
    public FakeValidateCreateEventCommand()
    {
        RuleFor(r => r.Price, _ => -10);
        RuleFor(r => r.Status, _ => (EventStatus)10);
        RuleFor(r => r.VenueId, _ => Guid.Empty);
        RuleFor(r => r.DurationMinutes, _ => 0);
        RuleFor(r => r.EventDate, _ => new DateTime());
    }
}
