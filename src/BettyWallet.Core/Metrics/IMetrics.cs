namespace BettyWallet.Core.Metrics;

/// <summary>
/// Operational numbers (counts, amounts), not individual records. Names come from <see cref="MetricNames"/>.
/// The app currently uses <see cref="NullMetrics"/>; a real implementation could map these onto
/// System.Diagnostics.Metrics, Prometheus or similar once that infrastructure exists.
/// </summary>
public interface IMetrics
{
    /// <summary>Adds to a counter, e.g. the number of bets placed.</summary>
    void Increment(string name, decimal by = 1);

    /// <summary>Records one measured value, e.g. the amount of a bet.</summary>
    void Record(string name, decimal value);
}
