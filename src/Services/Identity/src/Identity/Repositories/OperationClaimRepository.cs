using EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Data;

namespace EventPAM.Identity.Repositories;

public class OperationClaimRepository : EfRepositoryBase<OperationClaim, IdentityContext>, IOperationClaimRepository
{
    public OperationClaimRepository(IdentityContext context) : base(context)
    {

    }
}
