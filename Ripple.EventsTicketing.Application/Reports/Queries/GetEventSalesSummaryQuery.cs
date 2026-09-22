using MediatR;

namespace Ripple.EventsTicketing.Application.Reports.Queries;

public sealed record PricingTierSalesDto(
    Guid PricingTierId,
    string Name,
    decimal Price,
    int Capacity,
    int RemainingQuantity,
    int QuantitySold,
    decimal Revenue);

public sealed record EventSalesSummaryDto(
    Guid EventId,
    string EventName,
    string Venue,
    DateTimeOffset StartsAt,
    int TotalCapacity,
    int TicketsSold,
    decimal TotalRevenue,
    IReadOnlyCollection<PricingTierSalesDto> PricingTiers);

public sealed record GetEventSalesSummaryQuery(Guid EventId) : IRequest<EventSalesSummaryDto>;
