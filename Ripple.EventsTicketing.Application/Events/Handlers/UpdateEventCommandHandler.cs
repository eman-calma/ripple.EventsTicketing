using MediatR;
using Ripple.EventsTicketing.Application.Common.Exceptions;
using Ripple.EventsTicketing.Application.Events.Commands;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Infrastructure.Persistence;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Application.Events.Handlers;

public class UpdateEventCommandHandler: IRequestHandler<UpdateEventCommand>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdWithTiersAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException($"Event '{request.EventId}' was not found.");
        }

        if (await _eventRepository.HasSalesAsync(request.EventId, cancellationToken))
        {
            throw new ConflictException(
                $"Event '{eventEntity.Name}' cannot be updated because tickets have already been sold.");
        }

        eventEntity.Name = request.Name;
        eventEntity.Description = request.Description;
        eventEntity.Venue = request.Venue;
        eventEntity.StartsAt = request.StartsAt;
        eventEntity.TotalCapacity = request.TotalCapacity;
        eventEntity.UpdatedAtUtc = DateTimeOffset.UtcNow;

        foreach (var tier in eventEntity.PricingTiers.ToList())
        {
            eventEntity.PricingTiers.Remove(tier);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedEntity = await _eventRepository.GetByIdWithTiersAsync(request.EventId, cancellationToken);
        if (updatedEntity is null)
        {
            throw new NotFoundException($"Event '{request.EventId}' was not found.");
        }
        AddNewTiers(updatedEntity, request.PricingTiers);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void AddNewTiers(Event eventEntity, IReadOnlyCollection<UpdatePricingTierCommand> requestedTiers)
    {
        foreach (var requestedTier in requestedTiers)
        {
            var pricingTier = new PricingTier
            {
                Id = Guid.NewGuid(),
                EventId = eventEntity.Id,
                Name = requestedTier.Name,
                Price = requestedTier.Price,
                Capacity = requestedTier.Capacity,
                RemainingQuantity = requestedTier.Capacity
            };

            eventEntity.PricingTiers.Add(pricingTier);
        }
    }
}