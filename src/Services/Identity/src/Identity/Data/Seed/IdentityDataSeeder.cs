using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Constants;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.EFCore;
using EventPAM.Identity.Repositories;
using Microsoft.AspNetCore.Http;

namespace EventPAM.Identity.Data.Seed;

public class IdentityDataSeeder : IDataSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityDataSeeder(
        IUserRepository userRepository,
        IOperationClaimRepository operationClaimRepository,
        IUserOperationClaimRepository userOperationClaimRepository,
        IEventDispatcher eventDispatcher,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _operationClaimRepository = operationClaimRepository;
        _userOperationClaimRepository = userOperationClaimRepository;
        _eventDispatcher = eventDispatcher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        if (await _operationClaimRepository.AnyAsync(oc => oc.Name == GeneralOperationClaims.Admin) == false)
        {
            await _operationClaimRepository.AddAsync(new OperationClaim { Name = GeneralOperationClaims.Admin });
        }

        if (await _operationClaimRepository.AnyAsync(oc => oc.Name == GeneralOperationClaims.Customer) == false)
        {
            await _operationClaimRepository.AddAsync(new OperationClaim { Name = GeneralOperationClaims.Customer });
        }

        if (await _operationClaimRepository.AnyAsync(oc => oc.Name == GeneralOperationClaims.EventManager) == false)
        {
            await _operationClaimRepository.AddAsync(new OperationClaim { Name = GeneralOperationClaims.EventManager });
        }
    }

    private async Task SeedUsers()
    {
        if (await _userRepository.AnyAsync(u => u.Email == "admin@eventpam.com") == false)
        {
            var admin = InitialData.Users.First();

            var result = await _userRepository.AddAsync(admin);

            if (result is not null)
            {
                var adminOperationClaim = new UserOperationClaim
                {
                    UserId = admin.Id,
                    OperationClaimId = (await _operationClaimRepository.GetAsync(oc => oc.Name == GeneralOperationClaims.Admin))!.Id
                };

                await _userOperationClaimRepository.AddAsync(adminOperationClaim);

                await _eventDispatcher.SendAsync(new UserCreated(admin.Id, admin.FirstName + " " + admin.LastName));
            }
        }

        if (await _userRepository.AnyAsync(u => u.Email == "customer@eventpam.com") == false)
        {
            var customer = InitialData.Users.Single(u => u.Email == "customer@eventpam.com");

            var result = await _userRepository.AddAsync(customer);

            if (result is not null)
            {
                var customerOperationClaim = new UserOperationClaim
                {
                    UserId = customer.Id,
                    OperationClaimId = (await _operationClaimRepository.GetAsync(oc => oc.Name == GeneralOperationClaims.Customer))!.Id
                };

                await _userOperationClaimRepository.AddAsync(customerOperationClaim);

                await _eventDispatcher.SendAsync(new UserCreated(customer.Id, customer.FirstName + " " + customer.LastName));
            }
        }

        if (await _userRepository.AnyAsync(u => u.Email == "eventmanager@eventpam.com") == false)
        {
            var eventManager = InitialData.Users.Last();

            var result = await _userRepository.AddAsync(eventManager);

            if (result is not null)
            {
                var eventManagerOperationClaim = new UserOperationClaim
                {
                    UserId = eventManager.Id,
                    OperationClaimId = (await _operationClaimRepository.GetAsync(oc => oc.Name == GeneralOperationClaims.EventManager))!.Id
                };

                await _userOperationClaimRepository.AddAsync(eventManagerOperationClaim);

                await _eventDispatcher.SendAsync(new UserCreated(eventManager.Id, eventManager.FirstName + " " + eventManager.LastName));
            }
        }
    }
}
