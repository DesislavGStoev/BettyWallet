namespace BettyWallet.Core;

/// <summary>
/// A player's balance.
/// Synchronous because the wallet is in memory. A persistent or remote wallet would make these methods async
/// and give bet and win calls a transaction ID, so a retried request isn't charged or paid twice.
/// </summary>
public interface IWallet
{
    decimal Balance { get; }

    OperationResult Deposit(decimal amount);

    OperationResult Withdraw(decimal amount);

    /// <summary>Takes the stake before a round is played. Fails if the player can't cover it.</summary>
    OperationResult DebitBet(decimal amount);

    /// <summary>Pays out the result of a round. Throws on a negative amount (a programming error).</summary>
    void CreditWin(decimal amount);
}
