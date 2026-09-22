using Moq;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Tickets.Commands;
using Ripple.EventsTicketing.Application.Tickets.Handlers;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Domain.Enums;
using Ripple.EventsTicketing.Infrastructure.Persistence;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Tests.Tickets;

public class PurchaseTicketsCommandHandlerTests
{
    private static Event CreateEventWithPricingTier(int remainingQuantity, out Guid pricingTierId)
    {
        var eventId = Guid.NewGuid();
        pricingTierId = Guid.NewGuid();

        var eventEntity = new Event
        {
            Id = eventId,
            Name = "Concert",
            TotalCapacity = remainingQuantity
        };

        eventEntity.PricingTiers.Add(new PricingTier
        {
            Id = pricingTierId,
            EventId = eventId,
            Name = "General",
            Price = 50m,
            Capacity = remainingQuantity,
            RemainingQuantity = remainingQuantity
        });

        return eventEntity;
    }

    [Fact]
    public async Task Handle_Throws_ConflictException_When_Requested_Quantity_Exceeds_Remaining()
    {
        var eventEntity = CreateEventWithPricingTier(remainingQuantity: 2, out var pricingTierId);

        var eventRepository = new Mock<IEventRepository>();
        eventRepository
            .Setup(x => x.GetByIdWithTiersAsync(eventEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository
            .Setup(x => x.GetByIdempotencyKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketOrder?)null);

        var handler = new PurchaseTicketsCommandHandler(eventRepository.Object, ticketRepository.Object, Mock.Of<IUnitOfWork>());

        var command = new PurchaseTicketsCommand(
            eventEntity.Id,
            "user-1",
            "key-1",
            new[] { new PurchaseTicketItemRequest(pricingTierId, 3) });

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Decrements_RemainingQuantity_When_Purchase_Succeeds()
    {
        var eventEntity = CreateEventWithPricingTier(remainingQuantity: 5, out var pricingTierId);

        var eventRepository = new Mock<IEventRepository>();
        eventRepository
            .Setup(x => x.GetByIdWithTiersAsync(eventEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository
            .Setup(x => x.GetByIdempotencyKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketOrder?)null);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new PurchaseTicketsCommandHandler(eventRepository.Object, ticketRepository.Object, unitOfWork.Object);

        var command = new PurchaseTicketsCommand(
            eventEntity.Id,
            "user-1",
            "key-1",
            new[] { new PurchaseTicketItemRequest(pricingTierId, 2) });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(3, eventEntity.PricingTiers.Single().RemainingQuantity);
        Assert.Equal(100m, result.TotalAmount);
        ticketRepository.Verify(x => x.AddAsync(It.IsAny<TicketOrder>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Returns_Existing_Order_When_Idempotency_Key_Already_Used()
    {
        var eventEntity = CreateEventWithPricingTier(remainingQuantity: 5, out var pricingTierId);

        var existingOrder = new TicketOrder
        {
            Id = Guid.NewGuid(),
            EventId = eventEntity.Id,
            UserObjectId = "user-1",
            IdempotencyKey = "key-1",
            TotalAmount = 100m,
            Status = OrderStatus.Confirmed
        };
        existingOrder.Items.Add(new TicketOrderItem
        {
            Id = Guid.NewGuid(),
            PricingTierId = pricingTierId,
            Quantity = 2,
            UnitPrice = 50m,
            PricingTier = eventEntity.PricingTiers.Single()
        });

        var eventRepository = new Mock<IEventRepository>();
        var ticketRepository = new Mock<ITicketRepository>();
        ticketRepository
            .Setup(x => x.GetByIdempotencyKeyAsync("user-1", "key-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrder);

        var handler = new PurchaseTicketsCommandHandler(eventRepository.Object, ticketRepository.Object, Mock.Of<IUnitOfWork>());

        var command = new PurchaseTicketsCommand(
            eventEntity.Id,
            "user-1",
            "key-1",
            new[] { new PurchaseTicketItemRequest(pricingTierId, 2) });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(existingOrder.Id, result.Id);
        eventRepository.Verify(x => x.GetByIdWithTiersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        ticketRepository.Verify(x => x.AddAsync(It.IsAny<TicketOrder>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
