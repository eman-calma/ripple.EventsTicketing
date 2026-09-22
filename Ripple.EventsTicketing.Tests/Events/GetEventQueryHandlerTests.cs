using Moq;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Events.Handlers;
using Ripple.EventsTicketing.Application.Events.Queries;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Tests.Events;

public class GetEventQueryHandlerTests
{
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_Event_Does_Not_Exist()
    {
        var eventRepository = new Mock<IEventRepository>();
        eventRepository
            .Setup(x => x.GetByIdWithTiersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event?)null);

        var handler = new GetEventQueryHandler(eventRepository.Object);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetEventQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Returns_EventDto_When_Event_Exists()
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Concert",
            Venue = "Arena",
            TotalCapacity = 10
        };

        var eventRepository = new Mock<IEventRepository>();
        eventRepository
            .Setup(x => x.GetByIdWithTiersAsync(eventEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        var handler = new GetEventQueryHandler(eventRepository.Object);

        var result = await handler.Handle(new GetEventQuery(eventEntity.Id), CancellationToken.None);

        Assert.Equal(eventEntity.Id, result.Id);
        Assert.Equal(eventEntity.Name, result.Name);
    }
}
