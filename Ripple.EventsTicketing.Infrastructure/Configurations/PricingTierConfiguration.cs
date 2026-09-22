using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ripple.EventsTicketing.Domain.Entities;

namespace Ripple.EventsTicketing.Infrastructure.Persistence.Configurations;

public class PricingTierConfiguration : IEntityTypeConfiguration<PricingTier>
{
    public void Configure(EntityTypeBuilder<PricingTier> builder)
    {
        builder.ToTable("PricingTiers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => x.EventId);
    }
}