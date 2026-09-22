using Ripple.EventsTicketing.Application.Reports.Queries;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Application.Reports.Handlers;

internal static class EventSalesSummaryMapper
{
    public static EventSalesSummaryDto Map(Event eventEntity, IReadOnlyCollection<PricingTierSales> sales)
    {
        var salesByTier = sales.ToDictionary(x => x.PricingTierId);

        var pricingTiers = eventEntity.PricingTiers
            .Select(tier =>
            {
                salesByTier.TryGetValue(tier.Id, out var tierSales);

                return new PricingTierSalesDto(
                    tier.Id,
                    tier.Name,
                    tier.Price,
                    tier.Capacity,
                    tier.RemainingQuantity,
                    tierSales?.QuantitySold ?? 0,
                    tierSales?.Revenue ?? 0m);
            })
            .ToList();

        return new EventSalesSummaryDto(
            eventEntity.Id,
            eventEntity.Name,
            eventEntity.Venue,
            eventEntity.StartsAt,
            eventEntity.TotalCapacity,
            pricingTiers.Sum(x => x.QuantitySold),
            pricingTiers.Sum(x => x.Revenue),
            pricingTiers);
    }
}
