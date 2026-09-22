using Microsoft.EntityFrameworkCore;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Domain.Enums;
using Ripple.EventsTicketing.Infrastructure.Persistence;

namespace Ripple.EventsTicketing.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventsTicketingDbContext _dbContext;

        public EventRepository(EventsTicketingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Event?> GetByIdWithTiersAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Events.Include(x => x.PricingTiers).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Events
                                        .AsNoTracking()
                                        .Include(x => x.PricingTiers)
                                        .OrderBy(x => x.StartsAt)
                                        .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Event entity,CancellationToken cancellationToken)
        {
            await _dbContext.Events.AddAsync(entity, cancellationToken);
        }

        public void Remove(Event entity)
        {
            _dbContext.Events.Remove(entity);
        }

        public async Task<bool> HasSalesAsync(Guid eventId,CancellationToken cancellationToken)
        {
            return await _dbContext.TicketOrders.AnyAsync(x => x.EventId == eventId && x.Status == OrderStatus.Confirmed, cancellationToken);
        }
    }
}
