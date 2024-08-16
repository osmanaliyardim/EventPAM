using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPAM.Identity.Data.Configurations;

public class EmailAuthenticatorConfiguration : IEntityTypeConfiguration<EmailAuthenticator>
{
    public void Configure(EntityTypeBuilder<EmailAuthenticator> builder)
    {
        builder
            .ToTable("EmailAuthenticators")
            .HasKey(e => e.Id);

        builder.Property(e => e.UserId)
            .HasColumnName("UserId");

        builder.Property(e => e.ActivationKey)
            .HasColumnName("ActivationKey");

        builder.Property(e => e.IsVerified)
            .HasColumnName("IsVerified");

        builder.HasOne(e => e.User);
    }
}
