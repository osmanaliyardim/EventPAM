namespace EventPAM.BuildingBlocks.Mongo;

public class MongoUnitOfWork<TContext>(TContext context) : IMongoUnitOfWork<TContext>, ITransactionAble
    where TContext : MongoDbContext
{
    public TContext Context { get; } = context;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.BeginTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.RollbackTransaction(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.CommitTransactionAsync(cancellationToken);
    }

    public void Dispose() => Context.Dispose();
}
