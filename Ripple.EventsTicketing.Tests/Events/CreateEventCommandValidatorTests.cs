using Ripple.EventsTicketing.Application.Events.Commands;
using Ripple.EventsTicketing.Application.Events.Validators;

namespace Ripple.EventsTicketing.Tests.Events;

public class CreateEventCommandValidatorTests
{
    private readonly CreateEventCommandValidator _validator = new();

    [Fact]
    public void Invalid_When_PricingTier_Capacities_Do_Not_Sum_To_TotalCapacity()
    {
        var command = new CreateEventCommand(
            "Concert",
            "Description",
            "Venue",
            DateTimeOffset.UtcNow.AddDays(1),
            100,
            new[] { new PricingTierRequest("General", 50m, 50) });

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Valid_When_PricingTier_Capacities_Sum_To_TotalCapacity()
    {
        var command = new CreateEventCommand(
            "Concert",
            "Description",
            "Venue",
            DateTimeOffset.UtcNow.AddDays(1),
            100,
            new[] { new PricingTierRequest("General", 50m, 100) });

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}

