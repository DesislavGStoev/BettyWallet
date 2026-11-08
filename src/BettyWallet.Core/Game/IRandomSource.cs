namespace BettyWallet.Core.Game;

/// <summary>Source of randomness for the game, so tests can force outcomes and a certified RNG can be plugged in.</summary>
public interface IRandomSource
{
    /// <summary>Returns a random value from 0 (inclusive) up to 1 (exclusive).</summary>
    decimal NextDecimal();
}
