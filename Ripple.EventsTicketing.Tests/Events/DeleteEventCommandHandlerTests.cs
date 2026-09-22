using Moq;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Events.Commands;
using Ripple.EventsTicketing.Application.Events.Handlers;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Infrastructure.Persistence;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Tests.Events;

public class DeleteEventCommandHandlerTests
{
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_Event_Does_Not_Exist()
    {
        var eventRepository = new Mock<IEventRepository>();
        eventRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event?)null);

        var handler = new DeleteEventCommandHandler(eventRepository.Object, Mock.Of<IUnitOfWork>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new DeleteEventCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_ConflictException_When_Tickets_Already_Sold()
    {
        var eventId = Guid.NewGuid();
        var eventEntity = new Event { Id = eventId, Name = "Concert" };

        var eventRepository = new Mock<IEventRepository>();
        eventRepository.Setup(x => x.GetByIdAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(eventEntity);
        eventRepository.Setup(x => x.HasSalesAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeleteEventCommandHandler(eventRepository.Object, Mock.Of<IUnitOfWork>());

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new DeleteEventCommand(eventId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Removes_Event_When_No_Sales_Exist()
    {
        var eventId = Guid.NewGuid();
        var eventEntity = new Event { Id = eventId, Name = "Concert" };

        var eventRepository = new Mock<IEventRepository>();
        eventRepository.Setup(x => x.GetByIdAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(eventEntity);
        eventRepository.Setup(x => x.HasSalesAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new DeleteEventCommandHandler(eventRepository.Object, unitOfWork.Object);

        await handler.Handle(new DeleteEventCommand(eventId), CancellationToken.None);

        eventRepository.Verify(x => x.Remove(eventEntity), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
