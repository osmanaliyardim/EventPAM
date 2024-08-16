using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Event.Venues.Models;

namespace EventPAM.Event.Repositories;

public class VenueRepository : MongoRepository<VenueReadModel, Guid>, IVenueRepository
{
    public VenueRepository(EventReadDbContext context) : base(context) 
    {

    }
}
