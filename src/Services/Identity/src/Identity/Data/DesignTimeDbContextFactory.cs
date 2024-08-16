using EventPAM.BuildingBlocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventPAM.Identity.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityContext>();

        builder.UseNpgsql(EventPAMBase.Configs.IDENTITYDB_CONFIG)
            .UseSnakeCaseNamingConvention();

        return new IdentityContext(builder.Options);
    }
}
