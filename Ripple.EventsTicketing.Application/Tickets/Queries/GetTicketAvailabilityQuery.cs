using MediatR;

namespace Ripple.EventsTicketing.Application.Tickets.Queries;

public sealed record PricingTierAvailabilityDto(
    Guid PricingTierId,
    string Name,
    decimal Price,
    int Capacity,
    int RemainingQuantity);

public sealed record TicketAvailabilityDto(
    Guid EventId,
    string EventName,
    IReadOnlyCollection<PricingTierAvailabilityDto> PricingTiers);

public sealed record GetTicketAvailabilityQuery(Guid EventId) : IRequest<TicketAvailabilityDto>;
