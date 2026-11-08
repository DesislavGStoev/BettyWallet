using BettyWallet.Core;
using BettyWallet.Core.Game;

namespace BettyWallet.Tests.Core;

public class SlotGameTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5.5)]
    [InlineData(10)]
    public void Bets_within_limits_are_accepted(decimal bet)
    {
        var game = new SlotGame(TestSettings.Default(), new FakeRandomSource());

        Assert.Equal(OperationError.None, game.ValidateBet(bet));
    }

    [Fact]
    public void Bet_below_minimum_is_rejected()
    {
        var game = new SlotGame(TestSettings.Default(), new FakeRandomSource());

        Assert.Equal(OperationError.BetBelowMinimum, game.ValidateBet(0.99m));
    }

    [Fact]
    public void Bet_above_maximum_is_rejected()
    {
        var game = new SlotGame(TestSettings.Default(), new FakeRandomSource());

        Assert.Equal(OperationError.BetAboveMaximum, game.ValidateBet(10.01m));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.4999)]
    public void Rolls_in_the_first_half_lose(decimal outcomeRoll)
    {
        var game = new SlotGame(TestSettings.Default(), new FakeRandomSource(outcomeRoll, 0.5m));

        Assert.Equal(0m, game.Spin(10m));
    }

    [Theory]
    [InlineData(0.0, 10.00)] // x1: the lowest win pays back the bet
    [InlineData(0.5, 15.00)] // x1.5
    [InlineData(0.999, 19.99)] // just under x2
    public void Rolls_in_the_next_forty_percent_win_between_x1_and_x2(decimal multiplierRoll, decimal expectedWin)
    {
        var game = new SlotGame(TestSettings.Default(), new FakeRandomSource(0.5m, multiplierRoll));

        Assert.Equal(expectedWin, game.Spin(10m));
    }

    [Theory]
    [InlineData(0.0, 20.00)] // x2
    [InlineData(0.5, 60.00)] // x6
    [InlineData(0.999, 99.92)] // just under x10
    public void Rolls_in_the_last_ten_percent_win_between_x2_and_x10(decimal multiplierRoll, decimal expectedWin)
    {
        var game = new SlotGame(TestSettings.Default(), new FakeRandomSource(0.9m, multiplierRoll));

        Assert.Equal(expectedWin, game.Spin(10m));
    }
}
