using Ripple.EventsTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ripple.EventsTicketing.Infrastructure.Persistence
{
    public class EventsTicketingDbContext : DbContext
    {
        public EventsTicketingDbContext(
            DbContextOptions<EventsTicketingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events => Set<Event>();

        public DbSet<PricingTier> PricingTiers => Set<PricingTier>();

        public DbSet<TicketOrder> TicketOrders => Set<TicketOrder>();

        public DbSet<TicketOrderItem> TicketOrderItems =>
        Set<TicketOrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(EventsTicketingDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
