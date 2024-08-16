using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Ticketing.Ticketing.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EventPAM.Ticketing.Data;

public class TicketingReadDbContext : MongoDbContext
{
    public TicketingReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Tickets = GetCollection<TicketingReadModel>("ticketingreadmodel");
    }

    public IMongoCollection<TicketingReadModel> Tickets { get; }
}
