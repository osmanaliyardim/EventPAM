using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.Mongo;

public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IAggregate<TId>
{

}
