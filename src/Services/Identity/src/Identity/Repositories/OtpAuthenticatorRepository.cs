using EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Data;

namespace EventPAM.Identity.Repositories;

public class OtpAuthenticatorRepository : EfRepositoryBase<OtpAuthenticator, IdentityContext>, IOtpAuthenticatorRepository
{
    public OtpAuthenticatorRepository(IdentityContext context) : base(context)
    {

    }
}
