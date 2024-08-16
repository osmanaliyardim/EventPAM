namespace EventPAM.UnitTest.Event.Fakes;

public static class FakeEventUpdate
{
    public static void Generate(EventPAM.Event.Events.Models.Event @event)
    {
        @event.Update(@event.Id, @event.EventNumber, @event.VenueId, @event.DurationMinutes,
            @event.EventDate, @event.Status, @event.Price, @event.IsDeleted);
    }
}
