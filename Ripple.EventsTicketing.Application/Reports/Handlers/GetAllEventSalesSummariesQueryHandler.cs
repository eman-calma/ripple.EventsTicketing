using MediatR;
using Ripple.EventsTicketing.Application.Reports.Queries;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Application.Reports.Handlers;

public class GetAllEventSalesSummariesQueryHandler : IRequestHandler<GetAllEventSalesSummariesQuery, IReadOnlyCollection<EventSalesSummaryDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IReportsRepository _reportsRepository;

    public GetAllEventSalesSummariesQueryHandler(IEventRepository eventRepository, IReportsRepository reportsRepository)
    {
        _eventRepository = eventRepository;
        _reportsRepository = reportsRepository;
    }

    public async Task<IReadOnlyCollection<EventSalesSummaryDto>> Handle(GetAllEventSalesSummariesQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventRepository.GetAllAsync(cancellationToken);
        var sales = await _reportsRepository.GetPricingTierSalesAsync(eventId: null, cancellationToken);

        var salesByEvent = sales
            .GroupBy(x => x.EventId)
            .ToDictionary(g => g.Key, g => (IReadOnlyCollection<Infrastructure.Repositories.PricingTierSales>)g.ToList());

        return events
            .Select(eventEntity => EventSalesSummaryMapper.Map(
                eventEntity,
                salesByEvent.TryGetValue(eventEntity.Id, out var eventSales) ? eventSales : Array.Empty<Infrastructure.Repositories.PricingTierSales>()))
            .ToList();
    }
}
