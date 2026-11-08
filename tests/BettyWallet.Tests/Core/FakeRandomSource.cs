using BettyWallet.Core.Game;

namespace BettyWallet.Tests.Core;

/// <summary>Returns pre-set values in order, so tests can force a specific game outcome.</summary>
internal sealed class FakeRandomSource(params decimal[] values) : IRandomSource
{
    private readonly Queue<decimal> _values = new(values);

    public int Calls { get; private set; }

    public decimal NextDecimal()
    {
        Calls++;
        return _values.Dequeue();
    }
}
