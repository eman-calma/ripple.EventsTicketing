using Ripple.EventsTicketing.Domain.Entities;

namespace Ripple.EventsTicketing.Infrastructure.Repositories
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<Event?> GetByIdWithTiersAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken);

        Task AddAsync(Event entity,CancellationToken cancellationToken);

        void Remove(Event entity);

        Task<bool> HasSalesAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
