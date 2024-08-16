using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Hashing;
using MassTransit;

namespace EventPAM.Identity.Data.Seed;

public static class InitialData
{
    public static List<User> Users { get; }

    static InitialData()
    {
        HashingHelper.CreatePasswordHash("Pass@w0rd1", out byte[] paswordHash, out byte[] passwordSalt);

        Users =
        [
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Osman",
                LastName = "Yardim",
                Email = "admin@eventpam.com",
                PasswordHash = paswordHash,
                PasswordSalt = passwordSalt
            },
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Ali",
                LastName = "Veli",
                Email = "customer@eventpam.com",
                PasswordHash = paswordHash,
                PasswordSalt = passwordSalt
            },
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Ahmet",
                LastName = "Mehmet",
                Email = "eventmanager@eventpam.com",
                PasswordHash = paswordHash,
                PasswordSalt = passwordSalt
            }
        ];
    }
}
