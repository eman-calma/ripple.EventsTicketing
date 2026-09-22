using MediatR;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Tickets.Queries;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Application.Tickets.Handlers;

public class GetTicketAvailabilityQueryHandler : IRequestHandler<GetTicketAvailabilityQuery, TicketAvailabilityDto>
{
    private readonly IEventRepository _eventRepository;

    public GetTicketAvailabilityQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<TicketAvailabilityDto> Handle(GetTicketAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdWithTiersAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException($"Event '{request.EventId}' was not found.");
        }

        return new TicketAvailabilityDto(eventEntity.Id,
                                        eventEntity.Name,
                                        eventEntity.PricingTiers
                                            .Select(x => new PricingTierAvailabilityDto(x.Id, x.Name, x.Price, x.Capacity, x.RemainingQuantity))
                                            .ToList());
    }
}
