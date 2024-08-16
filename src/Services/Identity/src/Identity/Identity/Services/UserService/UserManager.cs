using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Repositories;

namespace EventPAM.Identity.Identity.Services.UserService;

public class UserManager : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _userRepository.GetAsync(u => u.Email == email);

        return user;
    }

    public async Task<User> GetById(Guid id)
    {
        var user = await _userRepository.GetAsync(u => u.Id == id);

        return user!;
    }

    public async Task<User> Update(User user)
    {
        var updatedUser = await _userRepository.UpdateAsync(user);

        return updatedUser;
    }
}
