using BettyWallet.Core.Auditing;

namespace BettyWallet.Core.Metrics;

/// <summary>All metric names in one place, so callers never type them as loose strings.</summary>
public static class MetricNames
{
    public const string Deposits = "wallet.deposits";
    public const string DepositAmount = "wallet.deposits.amount";
    public const string Withdrawals = "wallet.withdrawals";
    public const string WithdrawalAmount = "wallet.withdrawals.amount";

    public const string BetsPlaced = "bets.placed";
    public const string BetAmount = "bets.amount";
    public const string WinAmount = "wins.amount";
    public const string RoundsWon = "rounds.won";
    public const string RoundsLost = "rounds.lost";

    /// <summary>Counter of rejected operations per reason, e.g. "withdraw.rejected.insufficientfunds".</summary>
    public static string Rejected(AuditOperation operation, OperationError reason) =>
        $"{operation}.rejected.{reason}".ToLowerInvariant();
}
