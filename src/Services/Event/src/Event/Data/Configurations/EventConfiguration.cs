using EventPAM.Event.Events.ValueObjects;
using EventPAM.Event.Venues.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPAM.Event.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Events.Models.Event>
{
    public void Configure(EntityTypeBuilder<Events.Models.Event> builder)
    {
        builder.ToTable(nameof(Events.Models.Event));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(@event => @event.Value, dbId => EventId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        builder.OwnsOne(
            x => x.EventNumber,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Events.Models.Event.EventNumber))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder
            .HasOne<Venue>()
            .WithMany()
            .HasForeignKey(p => p.VenueId)
            .IsRequired();

        builder.OwnsOne(
            x => x.DurationMinutes,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Events.Models.Event.DurationMinutes))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.Property(x => x.Status)
            .HasDefaultValue(Events.Enums.EventStatus.Unknown)
            .HasConversion(
                x => x.ToString(),
                x => (Events.Enums.EventStatus)Enum.Parse(typeof(Events.Enums.EventStatus), x));

        builder.OwnsOne(
            x => x.Price,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Events.Models.Event.Price))
                    .HasMaxLength(10)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.EventDate,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Events.Models.Event.EventDate))
                    .IsRequired();
            }
        );
    }
}
