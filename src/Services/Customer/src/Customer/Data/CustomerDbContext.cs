using System.Reflection;
using EventPAM.BuildingBlocks.EFCore;
using EventPAM.BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventPAM.Customer.Data;

public sealed class CustomerDbContext : AppDbContextBase
{
    public DbSet<Customers.Models.Customer> Customer => Set<Customers.Models.Customer>();

    public CustomerDbContext(DbContextOptions<CustomerDbContext> options,
        ICurrentUserProvider? currentUserProvider = null, ILogger<CustomerDbContext>? logger = null) :
        base(options, currentUserProvider, logger)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
