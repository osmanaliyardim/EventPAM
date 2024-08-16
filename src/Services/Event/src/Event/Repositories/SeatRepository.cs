using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Event.Seats.Models;

namespace EventPAM.Event.Repositories;

public class SeatRepository : MongoRepository<SeatReadModel, Guid>, ISeatRepository
{
    public SeatRepository(EventReadDbContext context) : base(context) 
    {

    }
}
