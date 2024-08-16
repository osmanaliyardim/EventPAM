using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Event.Events.Models;

namespace EventPAM.Event.Repositories;

public interface IEventRepository : IMongoRepository<EventReadModel, Guid> 
{ 

}
