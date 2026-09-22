using MediatR;

namespace Ripple.EventsTicketing.Application.Tickets.Commands;

public record PurchaseTicketItemRequest(
    Guid PricingTierId,
    int Quantity);

public record TicketOrderItemDto(
    Guid PricingTierId,
    string PricingTierName,
    int Quantity,
    decimal UnitPrice);

public record TicketOrderDto(
    Guid Id,
    Guid EventId,
    string UserObjectId,
    DateTimeOffset PurchasedAtUtc,
    decimal TotalAmount,
    string Status,
    IReadOnlyCollection<TicketOrderItemDto> Items);

public record PurchaseTicketsCommand(
    Guid EventId,
    string UserObjectId,
    string IdempotencyKey,
    IReadOnlyCollection<PurchaseTicketItemRequest> Items)
    : IRequest<TicketOrderDto>;
