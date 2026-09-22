using MediatR;

namespace Ripple.EventsTicketing.Application.Events.Commands;

public record UpdateEventCommand(
    Guid EventId,
    string Name,
    string Description,
    string Venue,
    DateTimeOffset StartsAt,
    int TotalCapacity,
    IReadOnlyCollection<UpdatePricingTierCommand> PricingTiers)
    : IRequest;

public record UpdatePricingTierCommand(
    Guid? Id,
    string Name,
    decimal Price,
    int Capacity);