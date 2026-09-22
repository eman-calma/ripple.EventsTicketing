using Ripple.EventsTicketing.Domain.Entities;

namespace Ripple.EventsTicketing.Infrastructure.Repositories
{
    public interface ITicketRepository
    {
        Task<TicketOrder?> GetByIdempotencyKeyAsync(string userObjectId, string idempotencyKey, CancellationToken cancellationToken);

        Task AddAsync(TicketOrder entity, CancellationToken cancellationToken);
    }
}
