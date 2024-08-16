using EventPAM.Event.Events.Enums;
using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Seats.Enums;
using EventPAM.Event.Seats.ValueObjects;
using EventPAM.Event.Venues.ValueObjects;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.UnitTest.Event.Common;

public static class DbContextFactory
{
    private static readonly Guid _venueId1 = NewId.NextGuid();
    private static readonly Guid _venueId2 = NewId.NextGuid();
    private static readonly Guid _venueId3 = NewId.NextGuid();
    private static readonly Guid _eventId1 = NewId.NextGuid();
    private static readonly Guid _eventId2 = NewId.NextGuid();

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
        var venues = new List<EventPAM.Event.Venues.Models.Venue>
        {
            EventPAM.Event.Venues.Models.Venue.Create(VenueId.Of(_venueId1), Name.Of("Stadium X"), Capacity.Of(100000)),
            EventPAM.Event.Venues.Models.Venue.Create(VenueId.Of(_venueId2), Name.Of("Concert Hall Y"), Capacity.Of(150000)),
            EventPAM.Event.Venues.Models.Venue.Create(VenueId.Of(_venueId3), Name.Of("Sport Complex Z"), Capacity.Of(50000))
        };

        context.Venues.AddRange(venues);

        var events = new List<EventPAM.Event.Events.Models.Event>
        {
            EventPAM.Event.Events.Models.Event.Create(EventId.Of(_eventId1),
            EventNumber.Of("EV001"), VenueId.Of(_venueId1), DurationMinutes.Of(120m),
            EventDate.Of(new DateTime(2023, 1, 31, 13, 0, 0)), EventStatus.Completed,
            Price.Of((decimal)8000)),

            EventPAM.Event.Events.Models.Event.Create(EventId.Of(_eventId2),
            EventNumber.Of("EV002"), VenueId.Of(_venueId3), DurationMinutes.Of(50m),
            EventDate.Of(new DateTime(2025, 1, 31, 13, 0, 0)), EventStatus.Unknown,
            Price.Of((decimal)5000))
        };

        context.Events.AddRange(events);

        var seats = new List<EventPAM.Event.Seats.Models.Seat>
        {
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1A"),
                SeatClass.Standard, EventId.Of(_eventId1)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1B"),
                SeatClass.Standard, EventId.Of(_eventId1)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1C"),
                SeatClass.Standard, EventId.Of(_eventId1)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2A"),
                SeatClass.Standard, EventId.Of(_eventId1)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2B"),
                SeatClass.Premium, EventId.Of(_eventId1)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2C"),
                SeatClass.Standard, EventId.Of(_eventId1)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1K"),
                SeatClass.Standard, EventId.Of(_eventId2)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1L"),
                SeatClass.Standard, EventId.Of(_eventId2)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2K"),
                SeatClass.Standard, EventId.Of(_eventId2)),
            EventPAM.Event.Seats.Models.Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2L"),
                SeatClass.Premium, EventId.Of(_eventId2))
        };

        context.Seats.AddRange(seats);

        context.SaveChanges();
    }

    public static void Destroy(EventDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
