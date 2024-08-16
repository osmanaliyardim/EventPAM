using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Event.Venues.Models;

namespace EventPAM.Event.Repositories;

public interface IVenueRepository : IMongoRepository<VenueReadModel, Guid> 
{ 

}
