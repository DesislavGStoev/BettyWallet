namespace BettyWallet.Core.Auditing;

/// <summary>
/// One attempted money operation, successful or not.
/// A real audit trail would also carry a round ID linking each bet to its win, and a player ID once there is more than one player.
/// </summary>
/// <param name="Amount">The amount as requested, even if it was rejected (e.g. a negative deposit).</param>
/// <param name="Error"><see cref="OperationError.None"/> when the operation succeeded, otherwise why it was rejected.</param>
/// <param name="BalanceAfter">The balance once the operation finished; unchanged when it was rejected.</param>
public sealed record AuditEntry(
    AuditOperation Operation,
    decimal Amount,
    OperationError Error,
    decimal BalanceAfter,
    DateTimeOffset Timestamp);
