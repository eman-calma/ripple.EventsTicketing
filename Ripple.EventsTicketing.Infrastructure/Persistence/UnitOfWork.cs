
namespace Ripple.EventsTicketing.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventsTicketingDbContext _dbContext;

        public UnitOfWork(EventsTicketingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            return new Transaction(transaction);
        }
    }
}
