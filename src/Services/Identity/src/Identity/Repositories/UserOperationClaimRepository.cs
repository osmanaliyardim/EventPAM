using EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Data;

namespace EventPAM.Identity.Repositories;

public class UserOperationClaimRepository 
    : EfRepositoryBase<UserOperationClaim, IdentityContext>, IUserOperationClaimRepository
{
    public UserOperationClaimRepository(IdentityContext context) : base(context)
    {

    }
}
