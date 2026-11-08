using BettyWallet.Core.Game;

namespace BettyWallet.Tests.Core;

public class GameSettingsTests
{
    [Fact]
    public void Task_rules_are_valid()
    {
        Assert.Empty(TestSettings.Default().Validate());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Min_bet_must_be_positive(decimal minBet)
    {
        var settings = TestSettings.Default();
        settings.MinBet = minBet;

        Assert.Contains(settings.Validate(), e => e.Contains("MinBet"));
    }

    [Fact]
    public void Max_bet_must_not_be_below_min_bet()
    {
        var settings = TestSettings.Default();
        settings.MaxBet = 0.5m;

        Assert.Contains(settings.Validate(), e => e.Contains("MaxBet"));
    }

    [Fact]
    public void At_least_one_outcome_is_required()
    {
        var settings = TestSettings.Default();
        settings.Outcomes = [];

        Assert.Contains(settings.Validate(), e => e.Contains("outcome"));
    }

    [Fact]
    public void Probabilities_must_add_up_to_one()
    {
        var settings = TestSettings.Default();
        settings.Outcomes[0].Probability = 0.6m;

        Assert.Contains(settings.Validate(), e => e.Contains("add up to 1"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void Each_probability_must_be_between_zero_and_one(decimal probability)
    {
        var settings = TestSettings.Default();
        settings.Outcomes[1].Probability = probability;

        Assert.Contains(settings.Validate(), e => e.Contains("Probability"));
    }

    [Fact]
    public void Multipliers_must_not_be_negative()
    {
        var settings = TestSettings.Default();
        settings.Outcomes[0].MinMultiplier = -1m;

        Assert.Contains(settings.Validate(), e => e.Contains("MinMultiplier"));
    }

    [Fact]
    public void Max_multiplier_must_not_be_below_min_multiplier()
    {
        var settings = TestSettings.Default();
        settings.Outcomes[2].MaxMultiplier = 1m;

        Assert.Contains(settings.Validate(), e => e.Contains("MaxMultiplier"));
    }
}
