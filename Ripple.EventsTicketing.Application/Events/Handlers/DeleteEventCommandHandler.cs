using MediatR;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Events.Commands;
using Ripple.EventsTicketing.Infrastructure.Persistence;
using Ripple.EventsTicketing.Infrastructure.Repositories;


namespace Ripple.EventsTicketing.Application.Events.Handlers;

public class DeleteEventCommandHandler: IRequestHandler<DeleteEventCommand>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventCommandHandler(IEventRepository eventRepository,IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException($"Event '{request.EventId}' was not found.");
        }

        //check if the event has already sold tickets
        var hasSales = await _eventRepository.HasSalesAsync(request.EventId, cancellationToken);

        if (hasSales)
        {
            throw new ConflictException("The event cannot be deleted because tickets have already been sold.");
        }

        _eventRepository.Remove(eventEntity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}