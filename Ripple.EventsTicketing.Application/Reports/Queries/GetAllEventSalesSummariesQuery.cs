using MediatR;

namespace Ripple.EventsTicketing.Application.Reports.Queries;

public sealed record GetAllEventSalesSummariesQuery : IRequest<IReadOnlyCollection<EventSalesSummaryDto>>;
