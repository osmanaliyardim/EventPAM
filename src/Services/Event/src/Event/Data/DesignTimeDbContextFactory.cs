using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventPAM.Event.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EventDbContext>
{
    public EventDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<EventDbContext>();

        builder.UseNpgsql(Configs.EVENTDB_CONFIG)
            .UseSnakeCaseNamingConvention();

        return new EventDbContext(builder.Options);
    }
}
