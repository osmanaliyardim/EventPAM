using EventPAM.Event.Venues.Models;
using EventPAM.Event.Venues.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPAM.Event.Data.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {

        builder.ToTable(nameof(Venue));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(venueId => venueId.Value, dbId => VenueId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        builder.OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Venue.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.Capacity,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Venue.Capacity))
                    .HasMaxLength(6)
                    .IsRequired();
            }
        );
    }
}
