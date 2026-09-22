using MediatR;

namespace Ripple.EventsTicketing.Application.Events.Commands;

public record DeleteEventCommand(Guid EventId): IRequest;