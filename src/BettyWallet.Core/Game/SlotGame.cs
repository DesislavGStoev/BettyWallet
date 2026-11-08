using System.Collections.Immutable;

namespace BettyWallet.Core.Game;

/// <summary>
/// A game of chance: picks an outcome by its probability, then a multiplier within that outcome's range.
/// Knows nothing about balances; the wallet handles money.
/// </summary>
public sealed class SlotGame : IGame
{
    private readonly ImmutableArray<Outcome> _outcomes;
    private readonly IRandomSource _random;

    public SlotGame(GameSettings settings, IRandomSource random)
    {
        var errors = settings.Validate();
        if (errors.Count > 0)
        {
            throw new ArgumentException($"Invalid game settings: {string.Join(" ", errors)}", nameof(settings));
        }

        // Keep a read-only copy of the validated rules. GameSettings is mutable so configuration can bind to it,
        // but later changes to that object must not reach a game that is already running.
        MinBet = settings.MinBet;
        MaxBet = settings.MaxBet;
        _outcomes = AssignRollRanges(settings.Outcomes);
        _random = random;
    }

    public decimal MinBet { get; }

    public decimal MaxBet { get; }

    public OperationError ValidateBet(decimal betAmount)
    {
        if (betAmount < MinBet)
        {
            return OperationError.BetBelowMinimum;
        }

        if (betAmount > MaxBet)
        {
            return OperationError.BetAboveMaximum;
        }

        return OperationError.None;
    }

    public decimal Spin(decimal betAmount)
    {
        if (ValidateBet(betAmount) != OperationError.None)
        {
            throw new ArgumentOutOfRangeException(nameof(betAmount), betAmount, "Bet is outside the game limits.");
        }

        var outcome = PickOutcome(NextRoll());

        // A second roll places the multiplier evenly within the outcome's range, e.g. 0.5 -> halfway between x2 and x10 = x6.
        var multiplier = outcome.MinMultiplier + (outcome.MaxMultiplier - outcome.MinMultiplier) * NextRoll();

        // Rounding down means the house never pays out a fraction of a cent more than the odds allow.
        return decimal.Round(betAmount * multiplier, 2, MidpointRounding.ToZero);
    }

    /// <summary>
    /// A roll outside 0 to 1 would pick the wrong outcome or a multiplier outside its range,
    /// so a faulty random source stops the round instead of paying out the wrong amount.
    /// </summary>
    private decimal NextRoll()
    {
        var roll = _random.NextDecimal();
        if (roll is < 0m or >= 1m)
        {
            throw new InvalidOperationException($"Random source returned {roll}; expected a value from 0 up to (not including) 1.");
        }

        return roll;
    }

    /// <summary>
    /// Gives each outcome a slice of the range 0 to 1, as wide as its probability, placed one after another:
    /// <code>
    /// 0 ──── Lose ──── 0.5 ──── Win ──── 0.9 ── Big win ── 1
    ///      (50% wide)        (40% wide)      (10% wide)
    /// </code>
    /// A roll is equally likely to land anywhere from 0 to 1, so it lands in each slice with that outcome's probability.
    /// </summary>
    private static ImmutableArray<Outcome> AssignRollRanges(IEnumerable<GameOutcome> outcomes)
    {
        var ranged = ImmutableArray.CreateBuilder<Outcome>();
        var rollFrom = 0m;
        foreach (var outcome in outcomes)
        {
            var rollTo = rollFrom + outcome.Probability;
            ranged.Add(new Outcome(rollFrom, rollTo, outcome.MinMultiplier, outcome.MaxMultiplier));
            rollFrom = rollTo;
        }

        return ranged.ToImmutable();
    }

    /// <summary>Returns the outcome whose slice contains the roll, e.g. 0.3 -> Lose, 0.5 -> Win, 0.95 -> Big win.</summary>
    private Outcome PickOutcome(decimal roll)
    {
        foreach (var outcome in _outcomes)
        {
            if (outcome.Contains(roll))
            {
                return outcome;
            }
        }

        // Can't happen: the slices cover 0 to 1 exactly (probabilities were validated to add up to 1) and the roll is below 1.
        // Throwing (rather than falling back to the last, biggest outcome) makes a bug visible instead of costly.
        throw new InvalidOperationException($"No outcome matched roll {roll}.");
    }

    /// <summary>An outcome with its slice of the roll range: from <see cref="RollFrom"/> (inclusive) up to <see cref="RollTo"/> (exclusive).</summary>
    private readonly record struct Outcome(decimal RollFrom, decimal RollTo, decimal MinMultiplier, decimal MaxMultiplier)
    {
        public bool Contains(decimal roll) => roll >= RollFrom && roll < RollTo;
    }
}
