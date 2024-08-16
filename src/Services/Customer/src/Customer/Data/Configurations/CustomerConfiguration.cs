using EventPAM.Customer.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPAM.Customer.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customers.Models.Customer>
{
    public void Configure(EntityTypeBuilder<Customers.Models.Customer> builder)
    {
        builder.ToTable(nameof(Customers.Models.Customer));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(customerId => customerId.Value, dbId => CustomerId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        builder.OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Customers.Models.Customer.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.Age,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Customers.Models.Customer.Age))
                    .HasMaxLength(3)
                    .IsRequired();
            }
        );

        builder.Property(x => x.CustomerType)
            .IsRequired()
            .HasDefaultValue(Customers.Enums.CustomerType.Unknown)
            .HasConversion(
                x => x.ToString(),
                x => (Customers.Enums.CustomerType)Enum.Parse(typeof(Customers.Enums.CustomerType), x));
    }
}
