using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        if (request.PricingTiers.Count == 0 || request.PricingTiers == null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.PricingTiers), "An event must have at least one pricing tier.")
            });
        }


        var existingTierIds = eventEntity.PricingTiers.Select(x => x.Id).ToList();
        var requestedTiers = request.PricingTiers.Select(x => x.Id).ToList();


        //Get existing tier id that is not in request. This will be deleted.
        var existingTierNotInRequest = existingTierIds
                    .Where(t1 => !requestedTiers.Select(t2 => t2).Contains(t1))
                    .Select(t1 => t1).ToList();


        if (existingTierNotInRequest.Count() > 0)
        RemoveTiersNotInRequest(eventEntity, existingTierNotInRequest);
        UpsertRequestedTiers(eventEntity, request.PricingTiers);

        eventEntity.Name = request.Name;
        eventEntity.Description = request.Description;
        eventEntity.Venue = request.Venue;
        eventEntity.StartsAt = request.StartsAt;
        eventEntity.TotalCapacity = request.TotalCapacity;
        eventEntity.UpdatedAtUtc = DateTimeOffset.UtcNow;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException($"Event '{eventEntity.Name}' was modified by another user. Please reload and try again.");
        }
    }

    private static void RemoveTiersNotInRequest(Event eventEntity, IEnumerable<Guid> existingTierNotInRequest)
    {
        var tiersToRemove = eventEntity.PricingTiers
            .Where(tier => existingTierNotInRequest.Contains(tier.Id))
            .ToList();

        foreach (var tier in tiersToRemove)
        {
            eventEntity.PricingTiers.Remove(tier);
        }
    }

    private static void UpsertRequestedTiers(Event eventEntity, IReadOnlyCollection<UpdatePricingTierCommand> requestedTiers)
    {
        foreach (var requestedTier in requestedTiers)
        {
            var existingTier = eventEntity.PricingTiers.FirstOrDefault(x => x.Id == requestedTier.Id);

            if (existingTier is not null)
            {
                existingTier.Name = requestedTier.Name;
                existingTier.Price = requestedTier.Price;
                existingTier.Capacity = requestedTier.Capacity;
                existingTier.RemainingQuantity = requestedTier.Capacity;
            }
            else
            {
                eventEntity.PricingTiers.Add(new PricingTier
                {
                    Id = requestedTier.Id,
                    EventId = eventEntity.Id,
                    Name = requestedTier.Name,
                    Price = requestedTier.Price,
                    Capacity = requestedTier.Capacity,
                    RemainingQuantity = requestedTier.Capacity
                });
            }
        }
    }
}