using System.Reflection;
using Microsoft.EntityFrameworkCore;
using EventPAM.BuildingBlocks.EFCore;
using Microsoft.Extensions.Logging;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.Web;

namespace EventPAM.Identity.Data;

public sealed class IdentityContext : AppDbContextBase
{
    public DbSet<User> Users { get; set; }

    public DbSet<OperationClaim> OperationClaims { get; set; }

    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }

    public IdentityContext(DbContextOptions<IdentityContext> options,
        ICurrentUserProvider? currentUserProvider = null, ILogger<IdentityContext>? logger = null) :
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
