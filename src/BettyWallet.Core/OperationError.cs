namespace BettyWallet.Core;

/// <summary>
/// Expected business failures. They are returned as values rather than thrown,
/// because they are a normal part of the player's flow, not exceptional situations.
/// </summary>
public enum OperationError
{
    None = 0,
    NonPositiveAmount,
    TooManyDecimals,
    InsufficientFunds,
    BetBelowMinimum,
    BetAboveMaximum,
}
