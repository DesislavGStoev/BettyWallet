namespace BettyWallet.Core.Game;

public sealed class GameOutcome
{
    public string Name { get; set; } = string.Empty;

    public decimal Probability { get; set; }

    public decimal MinMultiplier { get; set; }

    public decimal MaxMultiplier { get; set; }
}
