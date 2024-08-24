using EventPAM.Event.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.UnitTest.Event.Common;

public static class DbContextFactory
{
    public static EventDbContext Create()
    {
        var options = new DbContextOptionsBuilder<EventDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        var context = new EventDbContext(options, null);

        // Seeding data
        EventDataSeeder(context);

        return context;
    }

    private static void EventDataSeeder(EventDbContext context)
    {
        var venues = InitialData.Venues;
        context.Venues.AddRange(venues);

        var events = InitialData.Events;
        context.Events.AddRange(events);

        var seats = InitialData.Seats;
        context.Seats.AddRange(seats);

        context.SaveChanges();
    }

    public static void Destroy(EventDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
