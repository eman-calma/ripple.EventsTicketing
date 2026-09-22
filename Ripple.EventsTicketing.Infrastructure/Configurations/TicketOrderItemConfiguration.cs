using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ripple.EventsTicketing.Domain.Entities;

namespace Ripple.EventsTicketing.Infrastructure.Configurations;

public class TicketOrderItemConfiguration : IEntityTypeConfiguration<TicketOrderItem>
{
    public void Configure(
        EntityTypeBuilder<TicketOrderItem> builder)
    {
        builder.ToTable("TicketOrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.PricingTierId);

        // Avoid multiple cascade paths to TicketOrderItems (via TicketOrder and via PricingTier).
        builder.HasOne(x => x.PricingTier)
            .WithMany()
            .HasForeignKey(x => x.PricingTierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


