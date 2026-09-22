
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ripple.EventsTicketing.Domain.Entities;

namespace Ripple.EventsTicketing.Infrastructure.Persistence.Configurations;

public class TicketOrderConfiguration : IEntityTypeConfiguration<TicketOrder>
{
    public void Configure(EntityTypeBuilder<TicketOrder> builder)
    {
        builder.ToTable("TicketOrders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserObjectId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new
        {
            x.UserObjectId,
            x.IdempotencyKey
        }).IsUnique();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.TicketOrder)
            .HasForeignKey(x => x.TicketOrderId);
    }
}