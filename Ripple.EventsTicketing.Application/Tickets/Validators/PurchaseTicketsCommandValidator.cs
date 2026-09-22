using FluentValidation;
using Ripple.EventsTicketing.Application.Tickets.Commands;

namespace Ripple.EventsTicketing.Application.Tickets.Validators;

public class PurchaseTicketsCommandValidator : AbstractValidator<PurchaseTicketsCommand>
{
    public PurchaseTicketsCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty();

        RuleFor(x => x.UserObjectId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Items)
            .NotEmpty();

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.PricingTierId)
                    .NotEmpty();

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0);
            });

        RuleFor(x => x.Items)
            .Must(items => items.Select(x => x.PricingTierId).Distinct().Count() == items.Count)
            .WithMessage("Duplicate pricing tier entries are not allowed in a single purchase.")
            .When(x => x.Items.Count > 0);
    }
}
