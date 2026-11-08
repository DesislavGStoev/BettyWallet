using System.Globalization;

namespace BettyWallet.ConsoleApp.Commands;

public static class CommandParser
{
    private static readonly Dictionary<string, ActionType> Actions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["deposit"] = ActionType.Deposit,
        ["withdraw"] = ActionType.Withdraw,
        ["bet"] = ActionType.Bet,
        ["exit"] = ActionType.Exit,
    };

    // A plain number with "." as the decimal separator, whatever the machine's culture.
    // The sign is allowed so "-5" reaches the wallet and gets a clear "must be positive" message.
    private const NumberStyles AmountStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    public static ParseResult Parse(string? input)
    {
        var parts = input?.TrimStart('﻿').Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries) ?? [];
        if (parts.Length == 0)
        {
            return ParseResult.Failure(ParseError.Empty);
        }

        if (!Actions.TryGetValue(parts[0], out var type))
        {
            return ParseResult.Failure(ParseError.UnknownAction, parts[0]);
        }

        if (type == ActionType.Exit)
        {
            return parts.Length == 1
                ? ParseResult.Success(new PlayerAction(type))
                : ParseResult.Failure(ParseError.TooManyArguments);
        }

        if (parts.Length == 1)
        {
            return ParseResult.Failure(ParseError.MissingAmount, parts[0]);
        }

        if (parts.Length > 2)
        {
            return ParseResult.Failure(ParseError.TooManyArguments);
        }

        return decimal.TryParse(parts[1], AmountStyle, CultureInfo.InvariantCulture, out var amount)
            ? ParseResult.Success(new PlayerAction(type, amount))
            : ParseResult.Failure(ParseError.InvalidAmount, parts[1]);
    }
}
