namespace Ripple.EventsTicketing.Infrastructure.Persistence
{
    public interface ITransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
