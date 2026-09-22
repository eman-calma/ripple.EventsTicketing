//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.EntityFrameworkCore;
//using Ripple.EventsTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ripple.EventsTicketing.Domain.Entities;

namespace Ripple.EventsTicketing.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Venue)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.TotalCapacity)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasMany(x => x.PricingTiers)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}