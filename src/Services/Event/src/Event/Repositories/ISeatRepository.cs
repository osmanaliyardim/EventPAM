using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Event.Seats.Models;

namespace EventPAM.Event.Repositories;

public interface ISeatRepository : IMongoRepository<SeatReadModel, Guid> 
{ 

}
