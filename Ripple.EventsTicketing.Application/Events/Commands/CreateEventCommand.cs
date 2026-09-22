using MediatR;
using Ripple.EventsTicketing.Application.Events.Queries;

namespace Ripple.EventsTicketing.Application.Events.Commands;

public record PricingTierRequest(
    string Name,
    decimal Price,
    int Capacity);

public record CreateEventCommand(
    string Name,
    string Description,
    string Venue,
    DateTimeOffset StartsAt,
    int TotalCapacity,
    IReadOnlyCollection<PricingTierRequest> PricingTiers)
    : IRequest<EventDto>;