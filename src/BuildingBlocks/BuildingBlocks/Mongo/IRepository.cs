using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.BuildingBlocks.Core.Persistence.Paging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace EventPAM.BuildingBlocks.Mongo;

public interface IReadRepository<TEntity, in TId>
    where TEntity : class, IAggregate<TId>
{
    Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IPaginate<TEntity>> GetAllPaginatedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>>? orderBy = null,
        int index = 0,
        int size = 10,
        CancellationToken cancellationToken = default
    );
}

public interface IWriteRepository<TEntity, in TId>
    where TEntity : class, IAggregate<TId>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(
        FilterDefinition<TEntity> filter,
        UpdateDefinition<TEntity> updateDefinition, 
        CancellationToken cancellationToken = default
    );

    Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);
}

public interface IRepository<TEntity, in TId> :
    IReadRepository<TEntity, TId>,
    IWriteRepository<TEntity, TId>,
    IDisposable
    where TEntity : class, IAggregate<TId>
{

}

public interface IRepository<TEntity> : IRepository<TEntity, long>
    where TEntity : class, IAggregate<long>
{

}
