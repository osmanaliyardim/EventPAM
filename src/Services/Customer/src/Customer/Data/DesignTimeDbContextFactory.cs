using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventPAM.Customer.Data;

public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<CustomerDbContext>
{
    public CustomerDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CustomerDbContext>();

        builder.UseNpgsql(Configs.CUSTOMERDB_CONFIG)
            .UseSnakeCaseNamingConvention();

        return new CustomerDbContext(builder.Options);
    }
}
