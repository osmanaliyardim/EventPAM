using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User = EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities.User;

namespace EventPAM.Identity.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable("Users")
            .HasKey(u => u.Id);

        builder
            .Property(u => u.Id)
            .HasColumnName("Id");

        builder
            .Property(u => u.FirstName)
            .HasColumnName("FirstName");

        builder
            .Property(u => u.LastName)
            .HasColumnName("LastName");

        builder
            .Property(u => u.Email)
            .HasColumnName("Email");

        builder
            .HasIndex(indexExpression: u => u.Email, name: "UK_Users_Email")
            .IsUnique();

        builder
            .Property(u => u.PasswordSalt)
            .HasColumnName("PasswordSalt");

        builder
            .Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash");

        builder
            .Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasDefaultValue(true);

        builder
            .Property(u => u.AuthenticatorType)
            .HasColumnName("AuthenticatorType");

        builder
            .HasMany(u => u.RefreshTokens);
    }
}
