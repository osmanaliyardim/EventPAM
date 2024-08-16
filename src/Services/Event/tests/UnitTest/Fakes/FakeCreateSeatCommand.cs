using EventPAM.Event.Data.Seed;
using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Seats.Enums;
using MassTransit;

namespace EventPAM.UnitTest.Event.Fakes;

public class FakeCreateSeatCommand : AutoFaker<CreateSeat>
{
    public FakeCreateSeatCommand()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.EventId, _ => InitialData.Events.First().Id);
        RuleFor(r => r.Class, _ => SeatClass.Standard);
    }
}
