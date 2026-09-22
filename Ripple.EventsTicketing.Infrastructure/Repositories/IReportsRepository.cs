namespace Ripple.EventsTicketing.Infrastructure.Repositories
{
    public sealed record PricingTierSales(
        Guid EventId,
        Guid PricingTierId,
        int QuantitySold,
        decimal Revenue);

    public interface IReportsRepository
    {
        Task<IReadOnlyList<PricingTierSales>> GetPricingTierSalesAsync(Guid? eventId, CancellationToken cancellationToken);
    }
}
