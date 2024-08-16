using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Event.Events.Models;

namespace EventPAM.Event.Repositories;

public class EventRepository : MongoRepository<EventReadModel, Guid>, IEventRepository
{
    public EventRepository(EventReadDbContext context) : base(context) 
    {

    }
}
