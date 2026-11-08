namespace BettyWallet.Core.Game;

public sealed class GameSettings
{
    public const string SectionName = "Game";

    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    /// <summary>Possible round results. Their probabilities must add up to exactly 1.</summary>
    public List<GameOutcome> Outcomes { get; set; } = [];

    /// <summary>Returns every rule violation, so a bad config can be fixed in one go.</summary>
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (MinBet <= 0)
        {
            errors.Add($"MinBet must be positive, but was {MinBet}.");
        }

        if (MaxBet < MinBet)
        {
            errors.Add($"MaxBet ({MaxBet}) must not be below MinBet ({MinBet}).");
        }

        if (Outcomes.Count == 0)
        {
            errors.Add("At least one outcome must be configured.");
            return errors;
        }

        foreach (var outcome in Outcomes)
        {
            var name = string.IsNullOrWhiteSpace(outcome.Name) ? "(unnamed)" : outcome.Name;

            if (outcome.Probability is <= 0 or > 1)
            {
                errors.Add($"Outcome '{name}': Probability must be greater than 0 and at most 1, but was {outcome.Probability}.");
            }

            if (outcome.MinMultiplier < 0)
            {
                errors.Add($"Outcome '{name}': MinMultiplier must not be negative, but was {outcome.MinMultiplier}.");
            }

            if (outcome.MaxMultiplier < outcome.MinMultiplier)
            {
                errors.Add($"Outcome '{name}': MaxMultiplier ({outcome.MaxMultiplier}) must not be below MinMultiplier ({outcome.MinMultiplier}).");
            }
        }

        var total = Outcomes.Sum(o => o.Probability);
        if (total != 1m)
        {
            errors.Add($"Outcome probabilities must add up to 1, but add up to {total}.");
        }

        return errors;
    }
}
