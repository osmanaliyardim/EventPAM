using System.Linq.Expressions;
using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.BuildingBlocks.Core.Persistance.Paging;
using EventPAM.BuildingBlocks.Core.Persistence.Paging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EventPAM.BuildingBlocks.Mongo;

public class MongoRepository<TEntity, TId> : IMongoRepository<TEntity, TId>
    where TEntity : class, IAggregate<TId>
{
    private readonly IMongoDbContext _context;
    protected readonly IMongoCollection<TEntity> DbSet;

    public MongoRepository(IMongoDbContext context)
    {
        _context = context;
        DbSet = _context.GetCollection<TEntity>();
    }

    public IMongoQueryable<TEntity> Query() => DbSet.AsQueryable();

    public void Dispose()
    {
        _context?.Dispose();
    }

    public async Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await Query().SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await Query().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().ToListAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindOneAndReplaceAsync(x => x.Id!.Equals(id), entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(
        FilterDefinition<TEntity> filter, 
        UpdateDefinition<TEntity> updateDefinition, 
        CancellationToken cancellationToken = default)
    {
        await DbSet.UpdateOneAsync(filter, updateDefinition, new UpdateOptions(), cancellationToken);
    }

    public async Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.DeleteOneAsync(e => entities.Any(i => e.Id!.Equals(i.Id)), cancellationToken);
    }

    public async Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
            => await DbSet.DeleteOneAsync(predicate, cancellationToken);

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.DeleteOneAsync(e => e.Id!.Equals(entity.Id), cancellationToken);
    }

    public async Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        await DbSet.DeleteOneAsync(e => e.Id!.Equals(id), cancellationToken);
    }

    public async Task<IPaginate<TEntity>> GetAllPaginatedAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IMongoQueryable<TEntity>, IOrderedMongoQueryable<TEntity>>? orderBy = null,  
        int index = 0, 
        int size = 10,
        CancellationToken cancellationToken = default)
    {
        var queryable = Query();

        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToReadPaginateAsync(index, size, from: 0, cancellationToken);

        return await queryable.ToReadPaginateAsync(index, size, from: 0, cancellationToken);
    }
}
