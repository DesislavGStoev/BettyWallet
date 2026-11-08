namespace BettyWallet.Core.Game;

/// <summary>
/// Default randomness based on <see cref="Random"/>. Fine for a demo; a regulated casino would
/// swap in a certified RNG through <see cref="IRandomSource"/>.
/// </summary>
public sealed class SystemRandomSource(Random random) : IRandomSource
{
    // Building the value from an integer keeps it exact and strictly below 1.
    // Converting NextDouble() to decimal can round 0.99999... up to 1.
    private const long Resolution = 1_000_000_000_000;

    public SystemRandomSource()
        : this(Random.Shared)
    {
    }

    public decimal NextDecimal() => random.NextInt64(Resolution) / (decimal)Resolution;
}
