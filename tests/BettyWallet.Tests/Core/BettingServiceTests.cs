using BettyWallet.Core;
using BettyWallet.Core.Game;

namespace BettyWallet.Tests.Core;

public class BettingServiceTests
{
    // Outcome rolls for the task's game (TestSettings): Lose [0, 0.5), Win [0.5, 0.9), Big win [0.9, 1).
    private const decimal LoseRoll = 0.1m;
    private const decimal BigWinRoll = 0.9m;

    [Fact]
    public void Winning_round_updates_balance_with_bet_and_win()
    {
        // Big win pays x2 to x10; a multiplier roll of 0.5 lands halfway: x6.
        var (service, wallet, _) = Setup(balance: 10m, outcomeRoll: BigWinRoll, multiplierRoll: 0.5m);

        var result = service.PlaceBet(5m);

        Assert.True(result.IsSuccess);
        Assert.Equal(30m, result.WinAmount);
        Assert.Equal(35m, wallet.Balance); // 10 - 5 + 30
    }

    [Fact]
    public void Losing_round_takes_the_bet()
    {
        var (service, wallet, _) = Setup(balance: 44.15m, outcomeRoll: LoseRoll);

        var result = service.PlaceBet(10m);

        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.WinAmount);
        Assert.Equal(34.15m, wallet.Balance);
    }

    [Theory]
    [InlineData(0.5, OperationError.BetBelowMinimum)]
    [InlineData(11, OperationError.BetAboveMaximum)]
    [InlineData(-5, OperationError.NonPositiveAmount)]
    [InlineData(1.555, OperationError.TooManyDecimals)]
    public void Invalid_bet_is_rejected_without_playing(decimal bet, OperationError expected)
    {
        var (service, wallet, random) = Setup(balance: 50m);

        var result = service.PlaceBet(bet);

        Assert.Equal(expected, result.Error);
        Assert.Equal(50m, wallet.Balance);
        Assert.Equal(0, random.Calls);
    }

    [Fact]
    public void Bet_larger_than_balance_is_rejected_without_playing()
    {
        var (service, wallet, random) = Setup(balance: 4m);

        var result = service.PlaceBet(5m);

        Assert.Equal(OperationError.InsufficientFunds, result.Error);
        Assert.Equal(4m, wallet.Balance);
        Assert.Equal(0, random.Calls);
    }

    /// <summary>
    /// A wallet holding <paramref name="balance"/>, betting on the task's slot game.
    /// The two rolls decide the round if it is played: the first picks the outcome, the second the multiplier within it.
    /// Tests that expect a bet to be rejected leave them out and check that the random source was never used.
    /// </summary>
    private static (BettingService Service, Wallet Wallet, FakeRandomSource Random) Setup(
        decimal balance,
        decimal outcomeRoll = LoseRoll,
        decimal multiplierRoll = 0m)
    {
        var wallet = TestFactory.NewWallet();
        wallet.Deposit(balance);

        var random = new FakeRandomSource(outcomeRoll, multiplierRoll);
        var game = new SlotGame(TestSettings.Default(), random);
        var service = TestFactory.NewBettingService(wallet, game);

        return (service, wallet, random);
    }
}
