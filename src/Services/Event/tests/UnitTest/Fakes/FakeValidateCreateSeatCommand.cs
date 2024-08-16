using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Seats.Enums;

namespace EventPAM.UnitTest.Event.Fakes;

public class FakeValidateCreateSeatCommand : AutoFaker<CreateSeat>
{
    public FakeValidateCreateSeatCommand()
    {
        RuleFor(r => r.SeatNumber, _ => null!);
        RuleFor(r => r.EventId, _ => Guid.Empty);
        RuleFor(r => r.Class, _ => (SeatClass)10);
    }
}
