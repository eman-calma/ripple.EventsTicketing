using MediatR;
using Ripple.EventsTicketing.Application.Events.Queries;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Application.Events.Handlers;

public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, IReadOnlyCollection<EventDto>>
{
    private readonly IEventRepository _eventRepository;

    public GetAllEventsQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IReadOnlyCollection<EventDto>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventRepository.GetAllAsync(cancellationToken);

        return events.Select(eventEntity => new EventDto(
                        eventEntity.Id,
                        eventEntity.Name,
                        eventEntity.Description,
                        eventEntity.Venue,
                        eventEntity.StartsAt,
                        eventEntity.TotalCapacity,
                        eventEntity.PricingTiers
                            .Select(tier => new PricingTierDto(
                                tier.Id,
                                tier.Name,
                                tier.Price,
                                tier.Capacity,
                                tier.RemainingQuantity))
                            .ToList()))
                    .ToList();
    }
}
