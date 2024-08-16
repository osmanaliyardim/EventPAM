using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Event.Events.Models;
using EventPAM.Event.Seats.Models;
using EventPAM.Event.Venues.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EventPAM.Event.Data;

public class EventReadDbContext : MongoDbContext
{
    public EventReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Events = GetCollection<EventReadModel>("eventreadmodel");
        Venues = GetCollection<VenueReadModel>("venuereadmodel");
        Seats = GetCollection<SeatReadModel>("seatreadmodel");
    }

    public IMongoCollection<EventReadModel> Events { get; }

    public IMongoCollection<VenueReadModel> Venues { get; }

    public IMongoCollection<SeatReadModel> Seats { get; }
}
