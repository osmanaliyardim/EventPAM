using EventPAM.Event.Events.Enums;
using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Seats.Enums;
using EventPAM.Event.Seats.Models;
using EventPAM.Event.Seats.ValueObjects;
using EventPAM.Event.Venues.Models;
using EventPAM.Event.Venues.ValueObjects;
using MassTransit;

namespace EventPAM.Event.Data.Seed;

public static class InitialData
{
    public static List<Venue> Venues { get; }

    public static List<Seat> Seats { get; }

    public static List<Events.Models.Event> Events { get; }

    static InitialData()
    {
        Venues =
        [
            Venue.Create(
                VenueId.Of(new Guid("3c5c0000-97c6-fc34-fcd3-08db322230c8")), 
                Name.Of("Stadium X"), Capacity.Of(100000), 
                new Address("Turkiye", "Izmir", "Ataturk St.", "35500")),
            Venue.Create(
                VenueId.Of(new Guid("3c5c0000-97c6-fc34-2e04-08db322230c9")), 
                Name.Of("Concert Hall Y"), Capacity.Of(150000), 
                new Address("Turkiye", "Istanbul", "Fevzi Cakmak Avenue St.", "35450")),
            Venue.Create(
                VenueId.Of(new Guid("3c5c0000-97c6-fc34-2e11-08db322230c9")), 
                Name.Of("Sport Complex Z"), Capacity.Of(50000), 
                new Address("Turkiye", "Ankara", "Enver Pasha St.", "35400"))
        ];

        Events =
        [
            Event.Events.Models.Event.Create(EventId.Of(new Guid("4c5c0000-97c6-fc34-2eb9-08db322230c9")),
            EventNumber.Of("EV001"), VenueId.Of(Venues.First().Id.Value), DurationMinutes.Of(120m),
            EventDate.Of(new DateTime(2023, 1, 31, 13, 0, 0)), EventStatus.Completed,
            Price.Of((decimal)8000)),

            Event.Events.Models.Event.Create(EventId.Of(new Guid("2c5c0000-97c6-fc34-2eb9-08db322230c8")),
            EventNumber.Of("EV002"), VenueId.Of(Venues.Last().Id.Value), DurationMinutes.Of(50m),
            EventDate.Of(new DateTime(2025, 1, 31, 13, 0, 0)), EventStatus.Unknown,
            Price.Of((decimal)5000))
        ];

        Seats =
        [
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1A"),
                SeatClass.Standard, EventId.Of(Events.First().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1B"),
                SeatClass.Standard, EventId.Of(Events.First().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1C"),
                SeatClass.Standard, EventId.Of(Events.First().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2A"),
                SeatClass.Standard, EventId.Of(Events.First().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2B"),
                SeatClass.Premium, EventId.Of(Events.First().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2C"),
                SeatClass.Standard, EventId.Of(Events.First().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1K"),
                SeatClass.Standard, EventId.Of(Events.Last().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("1L"),
                SeatClass.Standard, EventId.Of(Events.Last().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2K"),
                SeatClass.Standard, EventId.Of(Events.Last().Id), true),
            Seat.Create(
                SeatId.Of(NewId.NextGuid()), SeatNumber.Of("2L"),
                SeatClass.Premium, EventId.Of(Events.Last().Id), true)
        ];
    }
}
