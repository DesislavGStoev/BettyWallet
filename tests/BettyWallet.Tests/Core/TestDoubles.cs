using BettyWallet.Core;
using BettyWallet.Core.Auditing;
using BettyWallet.Core.Game;
using BettyWallet.Core.Metrics;

namespace BettyWallet.Tests.Core;

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}

internal static class TestFactory
{
    public static readonly DateTimeOffset Now = new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

    public static Wallet NewWallet() =>
        new(new NullAuditLog(), new NullMetrics(), new FixedTimeProvider(Now));

    public static BettingService NewBettingService(IWallet wallet, IGame game) =>
        new(wallet, game, new NullAuditLog(), new NullMetrics(), new FixedTimeProvider(Now));
}
