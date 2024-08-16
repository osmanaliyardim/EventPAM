using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

namespace EventPAM.Identity.Identity.Services.UserService;

public interface IUserService
{
    public Task<User?> GetByEmail(string email);

    public Task<User> GetById(Guid id);

    public Task<User> Update(User user);
}
