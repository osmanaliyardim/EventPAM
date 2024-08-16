using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Seats.Enums;
using MassTransit;

namespace EventPAM.IntegrationTest.Event.Fakes;

public class FakeCreateSeatMongoCommand : AutoFaker<CreateSeatMongo>
{
    public FakeCreateSeatMongoCommand(Guid eventId)
    {
        RuleFor(r => r.SeatId, _ => NewId.NextGuid());
        RuleFor(r => r.EventId, _ => eventId);
        RuleFor(r => r.Class, _ => SeatClass.Standard);
        RuleFor(r => r.IsDeleted, _ => false);
    }
}
