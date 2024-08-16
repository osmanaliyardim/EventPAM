using EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Data;

namespace EventPAM.Identity.Repositories;

public class UserRepository : EfRepositoryBase<User, IdentityContext>, IUserRepository
{
    public UserRepository(IdentityContext context) : base(context)
    {

    }
}
