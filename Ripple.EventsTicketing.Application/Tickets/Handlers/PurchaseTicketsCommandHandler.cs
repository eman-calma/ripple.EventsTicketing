using MediatR;
using Microsoft.EntityFrameworkCore;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Tickets.Commands;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Domain.Enums;
using Ripple.EventsTicketing.Infrastructure.Persistence;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Application.Tickets.Handlers;

public class PurchaseTicketsCommandHandler : IRequestHandler<PurchaseTicketsCommand, TicketOrderDto>
{
    private readonly IEventRepository _eventRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseTicketsCommandHandler(
        IEventRepository eventRepository,
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketOrderDto> Handle(PurchaseTicketsCommand request, CancellationToken cancellationToken)
    {
        var existingOrder = await _ticketRepository.GetByIdempotencyKeyAsync(request.UserObjectId, request.IdempotencyKey, cancellationToken);

        if (existingOrder is not null)
        {
            return MapToDto(existingOrder);
        }

        var eventEntity = await _eventRepository.GetByIdWithTiersAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException($"Event '{request.EventId}' was not found.");
        }

        var order = new TicketOrder
        {
            Id = Guid.NewGuid(),
            EventId = eventEntity.Id,
            UserObjectId = request.UserObjectId,
            IdempotencyKey = request.IdempotencyKey,
            PurchasedAtUtc = DateTimeOffset.UtcNow,
            Status = OrderStatus.Confirmed
        };

        var itemDtos = new List<TicketOrderItemDto>();

        foreach (var item in request.Items)
        {
            var pricingTier = eventEntity.PricingTiers.FirstOrDefault(x => x.Id == item.PricingTierId);

            if (pricingTier is null)
            {
                throw new NotFoundException($"Pricing tier '{item.PricingTierId}' was not found for event '{request.EventId}'.");
            }

            // Check if the requested quantity exceeds the remaining quantity for the pricing tier
            if (pricingTier.RemainingQuantity < item.Quantity)
            {
                throw new ConflictException($"Only {pricingTier.RemainingQuantity} ticket(s) remaining for pricing tier '{pricingTier.Name}'.");
            }

            pricingTier.RemainingQuantity -= item.Quantity;

            order.Items.Add(new TicketOrderItem
            {
                Id = Guid.NewGuid(),
                PricingTierId = pricingTier.Id,
                Quantity = item.Quantity,
                UnitPrice = pricingTier.Price
            });

            itemDtos.Add(new TicketOrderItemDto(pricingTier.Id, pricingTier.Name, item.Quantity, pricingTier.Price));
        }

        order.TotalAmount = itemDtos.Sum(x => x.Quantity * x.UnitPrice);

        await _ticketRepository.AddAsync(order, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("Ticket availability changed while processing your purchase. Please try again.");
        }

        return new TicketOrderDto(order.Id,
                                order.EventId,
                                order.UserObjectId,
                                order.PurchasedAtUtc,
                                order.TotalAmount,
                                order.Status.ToString(),
                                itemDtos);
    }

    private static TicketOrderDto MapToDto(TicketOrder order)
    {
        return new TicketOrderDto(order.Id,
                                order.EventId,
                                order.UserObjectId,
                                order.PurchasedAtUtc,
                                order.TotalAmount,
                                order.Status.ToString(),
                                order.Items
                                    .Select(x => new TicketOrderItemDto(x.PricingTierId, x.PricingTier.Name, x.Quantity, x.UnitPrice))
                                    .ToList());
    }
}
