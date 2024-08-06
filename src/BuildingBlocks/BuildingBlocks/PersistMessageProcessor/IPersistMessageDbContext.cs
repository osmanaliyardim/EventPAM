using Microsoft.EntityFrameworkCore;

namespace EventPAM.BuildingBlocks.PersistMessageProcessor;

public interface IPersistMessageDbContext
{
    DbSet<PersistMessage> PersistMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default);
}
