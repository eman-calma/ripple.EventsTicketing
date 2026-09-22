
using MediatR;

namespace Ripple.EventsTicketing.Application.Events.Queries;

public record PricingTierDto(
    Guid Id,
    string Name,
    decimal Price,
    int Capacity,
    int RemainingQuantity);

public record EventDto(
    Guid Id,
    string Name,
    string Description,
    string Venue,
    DateTimeOffset StartsAt,
    int TotalCapacity,
    IReadOnlyCollection<PricingTierDto> PricingTiers);

public record GetEventQuery(Guid Id) : IRequest<EventDto>;