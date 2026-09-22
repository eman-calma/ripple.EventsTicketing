using MediatR;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Reports.Queries;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Application.Reports.Handlers;

public class GetEventSalesSummaryQueryHandler : IRequestHandler<GetEventSalesSummaryQuery, EventSalesSummaryDto>
{
    private readonly IEventRepository _eventRepository;
    private readonly IReportsRepository _reportsRepository;

    public GetEventSalesSummaryQueryHandler(IEventRepository eventRepository, IReportsRepository reportsRepository)
    {
        _eventRepository = eventRepository;
        _reportsRepository = reportsRepository;
    }

    public async Task<EventSalesSummaryDto> Handle(GetEventSalesSummaryQuery request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdWithTiersAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException($"Event '{request.EventId}' was not found.");
        }

        var sales = await _reportsRepository.GetPricingTierSalesAsync(request.EventId, cancellationToken);

        return EventSalesSummaryMapper.Map(eventEntity, sales);
    }
}
