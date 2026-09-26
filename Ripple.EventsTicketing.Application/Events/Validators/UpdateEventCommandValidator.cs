using FluentValidation;
using Ripple.EventsTicketing.Application.Events.Commands;

namespace Ripple.EventsTicketing.Application.Events.Validators;

public class UpdateEventCommandValidator: AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Venue)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.StartsAt)
            .NotEmpty();

        RuleFor(x => x.TotalCapacity)
            .GreaterThan(0);

        RuleFor(x => x.PricingTiers)
            .NotEmpty();

        RuleForEach(x => x.PricingTiers)
            .ChildRules(tier =>
            {
                tier.RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(100);

                tier.RuleFor(x => x.Price)
                    .GreaterThanOrEqualTo(0);

                tier.RuleFor(x => x.Capacity)
                    .GreaterThan(0);
            });

        RuleFor(x => x.PricingTiers)
            .Must(HaveUniqueIds)
            .WithMessage("Duplicate pricing tier IDs are not allowed.");

        RuleFor(x => x)
            .Must(command =>
                command.PricingTiers.Sum(x => x.Capacity)
                == command.TotalCapacity)
            .WithMessage("The sum of pricing tier capacities must equal the event total capacity.");
    }

    private static bool HaveUniqueIds(IReadOnlyCollection<UpdatePricingTierCommand> tiers)
    {
        var ids = tiers
            .Select(x => x.Id)
            .ToList();

        return ids.Count == ids.Distinct().Count();
    }
}