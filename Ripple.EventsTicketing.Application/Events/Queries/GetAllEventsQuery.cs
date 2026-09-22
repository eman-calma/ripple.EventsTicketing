using MediatR;

namespace Ripple.EventsTicketing.Application.Events.Queries;

public record GetAllEventsQuery : IRequest<IReadOnlyCollection<EventDto>>;
