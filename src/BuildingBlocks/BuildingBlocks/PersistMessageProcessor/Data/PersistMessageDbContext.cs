﻿using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.BuildingBlocks.EFCore;
using EventPAM.BuildingBlocks.PersistMessageProcessor.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Exception = System.Exception;
using IsolationLevel = System.Data.IsolationLevel;

namespace EventPAM.BuildingBlocks.PersistMessageProcessor.Data;

public class PersistMessageDbContext : DbContext, IPersistMessageDbContext
{
    private readonly ILogger<PersistMessageDbContext>? _logger;

    public PersistMessageDbContext(DbContextOptions<PersistMessageDbContext> options,
        ILogger<PersistMessageDbContext>? logger = null)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<PersistMessage> PersistMessages => Set<PersistMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new PersistMessageConfiguration());
        base.OnModelCreating(builder);
        builder.ToSnakeCaseTables();
    }

    public Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        
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

    public void CreatePersistMessageTable()
    {
        if (Database.GetPendingMigrations().Any())
        {
            throw new InvalidOperationException(Messages.CANNOT_CREATE_DB);
        }

        string createTableSql = @"
            create table if not exists persist_message (
            id uuid not null,
            data_type text,
            data text,
            created timestamp with time zone not null,
            retry_count integer not null,
            message_status text not null default 'InProgress'::text,
            delivery_type text not null default 'Outbox'::text,
            version bigint not null,
            constraint pk_persist_message primary key (id)
            )";

        Database.ExecuteSqlRaw(createTableSql);
    }

    private void OnBeforeSaving()
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<IVersion>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.Version++;
                        break;

                    case EntityState.Deleted:
                        entry.Entity.Version++;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(Messages.IVERSION_ERROR, ex);
        }
    }
}
