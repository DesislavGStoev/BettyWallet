using BettyWallet.Core.Auditing;
using BettyWallet.Core.Metrics;

namespace BettyWallet.Core;

/// <summary>
/// In-memory balance of a single player. Not thread-safe: a console session has one player
/// and one input loop. A real service would keep balances in durable storage instead.
/// </summary>
public sealed class Wallet(IAuditLog auditLog, IMetrics metrics, TimeProvider timeProvider) : IWallet
{
    public decimal Balance { get; private set; }

    public OperationResult Deposit(decimal amount)
    {
        var result = Credit(amount);
        Audit(AuditOperation.Deposit, amount, result);
        Count(AuditOperation.Deposit, amount, result, MetricNames.Deposits, MetricNames.DepositAmount);
        return result;
    }

    public OperationResult Withdraw(decimal amount)
    {
        var result = Debit(amount);
        Audit(AuditOperation.Withdraw, amount, result);
        Count(AuditOperation.Withdraw, amount, result, MetricNames.Withdrawals, MetricNames.WithdrawalAmount);
        return result;
    }

    /// <summary>
    /// Takes the stake before the round is played, so a round can never run without funds.
    /// </summary>
    public OperationResult DebitBet(decimal amount)
    {
        var result = Debit(amount);
        Audit(AuditOperation.Bet, amount, result);
        return result;
    }

    /// <summary>
    /// Pays out the result of a round.
    /// </summary>
    public void CreditWin(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Balance += amount;
        Audit(AuditOperation.Win, amount, OperationResult.Success);
    }

    private OperationResult Credit(decimal amount)
    {
        var error = Amount.Validate(amount);
        if (error != OperationError.None)
        {
            return OperationResult.Failure(error);
        }

        Balance += amount;
        return OperationResult.Success;
    }

    private OperationResult Debit(decimal amount)
    {
        var error = Amount.Validate(amount);
        if (error != OperationError.None)
        {
            return OperationResult.Failure(error);
        }

        if (amount > Balance)
        {
            return OperationResult.Failure(OperationError.InsufficientFunds);
        }

        Balance -= amount;
        return OperationResult.Success;
    }

    private void Audit(AuditOperation operation, decimal amount, OperationResult result) =>
        auditLog.Record(new AuditEntry(operation, amount, result.Error, Balance, timeProvider.GetUtcNow()));

    private void Count(AuditOperation operation, decimal amount, OperationResult result, string counterName, string amountName)
    {
        if (!result.IsSuccess)
        {
            metrics.Increment(MetricNames.Rejected(operation, result.Error));
            return;
        }

        metrics.Increment(counterName);
        metrics.Record(amountName, amount);
    }
}
