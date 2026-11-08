namespace BettyWallet.Core.Metrics;

/// <summary>Placeholder until metrics infrastructure exists: accepts measurements and discards them.</summary>
public sealed class NullMetrics : IMetrics
{
    public void Increment(string name, decimal by = 1)
    {
    }

    public void Record(string name, decimal value)
    {
    }
}
