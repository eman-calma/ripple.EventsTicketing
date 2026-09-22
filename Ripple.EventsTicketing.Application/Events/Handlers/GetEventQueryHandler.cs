using MediatR;
using Ripple.EventsTicketing.Application.Events.Queries;
using Ripple.EventsTicketing.Infrastructure.Repositories;
using Ripple.EventsTicketing.Application.Common.Exceptions;

namespace Ripple.EventsTicketing.Application.Events.Handlers;

public class GetEventQueryHandler : IRequestHandler<GetEventQuery, EventDto>
{
    private readonly IEventRepository _eventRepository;

    public GetEventQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventDto> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdWithTiersAsync(request.Id, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException(
                $"Event {request.Id} was not found.");
        }

        return new EventDto(eventEntity.Id,
                            eventEntity.Name,
                            eventEntity.Description,
                            eventEntity.Venue,
                            eventEntity.StartsAt,
                            eventEntity.TotalCapacity,
                            eventEntity.PricingTiers
                                .Select(x => new PricingTierDto(
                                    x.Id,
                                    x.Name,
                                    x.Price,
                                    x.Capacity,
                                    x.RemainingQuantity))
                                .ToList());
    }
}