using EventPAM.BuildingBlocks.EFCore;
using EventPAM.Event.Seats.Models;
using EventPAM.Event.Venues.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventPAM.Event.Data;

public sealed class EventDbContext : AppDbContextBase
{
    public DbSet<Events.Models.Event> Events => Set<Events.Models.Event>();

    public DbSet<Venue> Venues => Set<Venue>();

    public DbSet<Seat> Seats => Set<Seat>();

    public EventDbContext
        (DbContextOptions<EventDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<EventDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(EventRoot).Assembly);
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
