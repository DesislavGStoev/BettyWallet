namespace BettyWallet.ConsoleApp.Commands;

public enum ActionType
{
    Deposit,
    Withdraw,
    Bet,
    Exit,
}

public enum ParseError
{
    None = 0,
    Empty,
    UnknownAction,
    MissingAmount,
    InvalidAmount,
    TooManyArguments,
}

public sealed record PlayerAction(ActionType Type, decimal Amount = 0m);

/// <param name="Token">The piece of input that caused the error, for showing back to the player.</param>
public sealed record ParseResult(PlayerAction? Action, ParseError Error, string? Token = null)
{
    public static ParseResult Success(PlayerAction action) => new(action, ParseError.None);

    public static ParseResult Failure(ParseError error, string? token = null) => new(null, error, token);
}
