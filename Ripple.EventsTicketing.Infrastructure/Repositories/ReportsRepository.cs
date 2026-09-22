using Microsoft.EntityFrameworkCore;
using Ripple.EventsTicketing.Domain.Enums;
using Ripple.EventsTicketing.Infrastructure.Persistence;

namespace Ripple.EventsTicketing.Infrastructure.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly EventsTicketingDbContext _dbContext;

        public ReportsRepository(EventsTicketingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<PricingTierSales>> GetPricingTierSalesAsync(Guid? eventId, CancellationToken cancellationToken)
        {
            var query = _dbContext.TicketOrderItems.Where(item => item.TicketOrder.Status == OrderStatus.Confirmed);

            if (eventId.HasValue)
            {
                query = query.Where(item => item.TicketOrder.EventId == eventId.Value);
            }

            return await query
                .GroupBy(item => new { item.TicketOrder.EventId, item.PricingTierId })
                .Select(g => new PricingTierSales(
                    g.Key.EventId,
                    g.Key.PricingTierId,
                    g.Sum(x => x.Quantity),
                    g.Sum(x => x.Quantity * x.UnitPrice)))
                .ToListAsync(cancellationToken);
        }
    }
}
