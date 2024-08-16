using EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Data;

namespace EventPAM.Identity.Repositories;

public class EmailAuthenticatorRepository : EfRepositoryBase<EmailAuthenticator, IdentityContext>, IEmailAuthenticatorRepository
{
    public EmailAuthenticatorRepository(IdentityContext context) : base(context)
    {

    }
}
