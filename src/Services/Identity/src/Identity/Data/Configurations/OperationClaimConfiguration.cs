using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Constants;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPAM.Identity.Data.Configurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder
            .ToTable("OperationClaims")
            .HasKey(o => o.Id);

        builder
            .Property(o => o.Id)
            .HasColumnName("Id");

        builder
            .Property(o => o.Name)
            .HasColumnName("Name");

        builder
            .HasIndex(indexExpression: o => o.Name, name: "UK_OperationClaims_Name")
            .IsUnique();

        builder
            .HasData(GetSeeds());
    }

    private HashSet<OperationClaim> GetSeeds()
    {
        HashSet<OperationClaim> seeds =
        [
            new OperationClaim { Id = Guid.NewGuid(), Name = GeneralOperationClaims.Admin },
            new OperationClaim { Id = Guid.NewGuid(), Name = GeneralOperationClaims.EventManager },
            new OperationClaim { Id = Guid.NewGuid(), Name = GeneralOperationClaims.Customer },
        ];

        return seeds;
    }
}
