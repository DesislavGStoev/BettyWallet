using BettyWallet.Core.Auditing;
using BettyWallet.Core.Game;
using BettyWallet.Core.Metrics;

namespace BettyWallet.Core;

public readonly record struct BetResult(OperationError Error, decimal WinAmount)
{
    public bool IsSuccess => Error == OperationError.None;

    public static BetResult Failure(OperationError error) => new(error, 0m);
}

/// <summary>
/// Plays one round against the wallet: validate the bet, take the stake, spin, pay the win.
/// new balance = old balance - bet + win.
/// The wallet audits every money movement; this service only audits bets it rejects
/// before they reach the wallet, so nothing is recorded twice. It owns the round metrics.
/// </summary>
public sealed class BettingService(IWallet wallet, IGame game, IAuditLog auditLog, IMetrics metrics, TimeProvider timeProvider)
{
    public BetResult PlaceBet(decimal betAmount)
    {
        var error = Amount.Validate(betAmount);
        if (error == OperationError.None)
        {
            error = game.ValidateBet(betAmount);
        }

        if (error != OperationError.None)
        {
            auditLog.Record(new AuditEntry(AuditOperation.Bet, betAmount, error, wallet.Balance, timeProvider.GetUtcNow()));
            return Rejected(error);
        }

        var debit = wallet.DebitBet(betAmount);
        if (!debit.IsSuccess)
        {
            return Rejected(debit.Error);
        }

        var winAmount = game.Spin(betAmount);
        wallet.CreditWin(winAmount);

        metrics.Increment(MetricNames.BetsPlaced);
        metrics.Increment(winAmount > 0 ? MetricNames.RoundsWon : MetricNames.RoundsLost);
        metrics.Record(MetricNames.BetAmount, betAmount);
        metrics.Record(MetricNames.WinAmount, winAmount);

        return new BetResult(OperationError.None, winAmount);
    }

    private BetResult Rejected(OperationError error)
    {
        metrics.Increment(MetricNames.Rejected(AuditOperation.Bet, error));
        return BetResult.Failure(error);
    }
}
