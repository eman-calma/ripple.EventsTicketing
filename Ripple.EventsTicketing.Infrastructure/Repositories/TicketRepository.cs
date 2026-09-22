using Microsoft.EntityFrameworkCore;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Infrastructure.Persistence;

namespace Ripple.EventsTicketing.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly EventsTicketingDbContext _dbContext;

        public TicketRepository(EventsTicketingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TicketOrder?> GetByIdempotencyKeyAsync(string userObjectId, string idempotencyKey, CancellationToken cancellationToken)
        {
            return await _dbContext.TicketOrders.Include(x => x.Items)
                                                .ThenInclude(x => x.PricingTier)
                                                .FirstOrDefaultAsync(x => x.UserObjectId == userObjectId && x.IdempotencyKey == idempotencyKey,cancellationToken);
        }

        public async Task AddAsync(TicketOrder entity, CancellationToken cancellationToken)
        {
            await _dbContext.TicketOrders.AddAsync(entity, cancellationToken);
        }
    }
}
