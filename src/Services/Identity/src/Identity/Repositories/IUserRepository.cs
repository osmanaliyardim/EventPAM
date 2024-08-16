using EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

namespace EventPAM.Identity.Repositories;

public interface IUserRepository : IAsyncRepository<User>, IRepository<User> { }
