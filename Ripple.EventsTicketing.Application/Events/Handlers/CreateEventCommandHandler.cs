
using MediatR;
using Ripple.EventsTicketing.Application.Events.Commands;
using Ripple.EventsTicketing.Application.Events.Queries;
using Ripple.EventsTicketing.Domain.Entities;
using Ripple.EventsTicketing.Infrastructure.Repositories;
using Ripple.EventsTicketing.Infrastructure.Persistence;

namespace Ripple.EventsTicketing.Application.Events.Handlers;

public sealed class CreateEventCommandHandler :
    IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Venue = request.Venue,
            StartsAt = request.StartsAt,
            TotalCapacity = request.TotalCapacity,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        foreach (var tier in request.PricingTiers)
        {
            eventEntity.PricingTiers.Add(
                new PricingTier
                {
                    Id = Guid.NewGuid(),
                    Name = tier.Name,
                    Price = tier.Price,
                    Capacity = tier.Capacity,
                    RemainingQuantity = tier.Capacity
                });
        }

        await _eventRepository.AddAsync(eventEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EventDto(
            eventEntity.Id,
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