using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Seats.Enums;
using MassTransit;

namespace EventPAM.IntegrationTest.Event.Fakes;

public class FakeCreateSeatCommand : AutoFaker<CreateSeat>
{
    public FakeCreateSeatCommand(Guid eventId)
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.EventId, _ => eventId);
        RuleFor(r => r.Class, _ => SeatClass.Standard);
    }
}
