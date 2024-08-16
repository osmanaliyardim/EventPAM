using System.Collections.Immutable;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Exception = System.Exception;
using IsolationLevel = System.Data.IsolationLevel;

namespace EventPAM.BuildingBlocks.EFCore;

public abstract class AppDbContextBase : DbContext, IDbContext
{
    private readonly ICurrentUserProvider? _currentUserProvider;
    private readonly ILogger<AppDbContextBase>? _logger;
    private IDbContextTransaction _currentTransaction = default!;

    protected AppDbContextBase(
        DbContextOptions options, 
        ICurrentUserProvider? currentUserProvider = null, 
        ILogger<AppDbContextBase>? logger = null) 
            : base(options)
    {
        _currentUserProvider = currentUserProvider;
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

    }

    public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null) return;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null!;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null!;
        }
    }

    public Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default)
    {
        var strategy = CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                if (databaseValues == null)
                {
                    _logger!.LogError(Messages.DB_CONSISTENCY_WARNING);
                    throw;
                }

                // Refresh the original values to bypass next concurrency check
                entry.OriginalValues.SetValues(databaseValues);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregate>()
                .Where(x => x.Entity.DomainEvents.Any())
                    .Select(x => x.Entity)
                    .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToImmutableList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        return domainEvents;
    }

    private void OnBeforeSaving()
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<IAggregate>())
            {
                var isAuditable = entry.Entity.GetType().IsAssignableTo(typeof(IAggregate));
                var userId = _currentUserProvider?.GetCurrentUserId() ?? 0;

                if (isAuditable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.CreatedAt = DateTime.Now;
                            break;

                        case EntityState.Modified:
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.Version++;
                            break;

                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.IsDeleted = true;
                            entry.Entity.Version++;
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(Messages.IAGGREGATE_ERROR, ex);
        }
    }
}
