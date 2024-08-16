using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Venues.ValueObjects;

namespace EventPAM.UnitTest.Event.Fakes;

public static class FakeEventCreate
{
    public static EventPAM.Event.Events.Models.Event Generate()
    {
        var command = new FakeCreateEventCommand().Generate();

        return EventPAM.Event.Events.Models.Event.Create(EventId.Of(command.Id),
            EventNumber.Of(command.EventNumber), VenueId.Of(command.VenueId), DurationMinutes.Of(command.DurationMinutes),
            EventDate.Of(command.EventDate), command.Status, Price.Of(command.Price));
    }
}
