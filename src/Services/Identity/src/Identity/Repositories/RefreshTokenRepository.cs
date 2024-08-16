using EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Data;

namespace EventPAM.Identity.Repositories;

public class RefreshTokenRepository : EfRepositoryBase<RefreshToken, IdentityContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IdentityContext context) : base(context)
    {

    }
}
