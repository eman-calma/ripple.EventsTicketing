using Microsoft.EntityFrameworkCore.Storage;

namespace Ripple.EventsTicketing.Infrastructure.Persistence
{
    public class Transaction : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        public Transaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _transaction.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _transaction.RollbackAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }
    }
}
