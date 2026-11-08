using BettyWallet.Core.Game;

namespace BettyWallet.Tests.Core;

internal static class TestSettings
{
    /// <summary>The rules from the task: $1-$10 bets, 50% lose, 40% win x1-x2, 10% win x2-x10.</summary>
    public static GameSettings Default() => new()
    {
        MinBet = 1m,
        MaxBet = 10m,
        Outcomes =
        [
            new() { Name = "Lose", Probability = 0.5m, MinMultiplier = 0m, MaxMultiplier = 0m },
            new() { Name = "Win", Probability = 0.4m, MinMultiplier = 1m, MaxMultiplier = 2m },
            new() { Name = "Big win", Probability = 0.1m, MinMultiplier = 2m, MaxMultiplier = 10m },
        ],
    };
}
