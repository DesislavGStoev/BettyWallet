namespace BettyWallet.Core;

/// <summary>
/// Validation rules shared by every operation that takes an amount from the player.
/// </summary>
public static class Amount
{
    public static OperationError Validate(decimal amount)
    {
        if (amount <= 0)
        {
            return OperationError.NonPositiveAmount;
        }

        if (decimal.Round(amount, 2) != amount)
        {
            return OperationError.TooManyDecimals;
        }

        return OperationError.None;
    }
}
