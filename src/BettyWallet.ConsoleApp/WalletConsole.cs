using System.Globalization;
using BettyWallet.ConsoleApp.Commands;
using BettyWallet.Core;
using BettyWallet.Core.Game;

namespace BettyWallet.ConsoleApp;

/// <summary>
/// The player-facing loop: read an action, run it, print the result and balance.
/// Reads and writes through TextReader/TextWriter so whole sessions can be tested without a real console.
/// </summary>
public sealed class WalletConsole(IWallet wallet, BettingService bettingService, IGame game, TextReader input, TextWriter output)
{
    private const string Prompt = "Please, submit action:";
    private const string Goodbye = "Thank you for playing! Hope to see you again soon.";
    private const string AvailableActions = "Available actions: deposit <amount>, withdraw <amount>, bet <amount>, exit.";

    public void Run()
    {
        while (true)
        {
            output.WriteLine(Prompt);
            var line = input.ReadLine();
            if (line is null)
            {
                break; // input closed, treat like exit
            }

            var parsed = CommandParser.Parse(line);
            if (parsed.Action?.Type == ActionType.Exit)
            {
                break;
            }

            output.WriteLine(parsed.Action is null ? Describe(parsed) : Execute(parsed.Action));
            output.WriteLine();
        }

        output.WriteLine(Goodbye);
    }

    private string Execute(PlayerAction action) => action.Type switch
    {
        ActionType.Deposit => Report(wallet.Deposit(action.Amount), $"Your deposit of {Money(action.Amount)} was successful."),
        ActionType.Withdraw => Report(wallet.Withdraw(action.Amount), $"Your withdrawal of {Money(action.Amount)} was successful."),
        ActionType.Bet => Report(bettingService.PlaceBet(action.Amount)),
        _ => throw new ArgumentOutOfRangeException(nameof(action), action.Type, "Unhandled action."),
    };

    private string Report(OperationResult result, string successMessage) =>
        WithBalance(result.IsSuccess ? successMessage : Describe(result.Error));

    private string Report(BetResult result)
    {
        if (!result.IsSuccess)
        {
            return WithBalance(Describe(result.Error));
        }

        return WithBalance(result.WinAmount > 0 ? $"Congrats - you won {Money(result.WinAmount)}!" : "No luck this time!");
    }

    private string Describe(OperationError error) => error switch
    {
        OperationError.NonPositiveAmount => "The amount must be a positive number.",
        OperationError.TooManyDecimals => "The amount can have at most 2 decimal places.",
        OperationError.InsufficientFunds => "Insufficient funds.",
        OperationError.BetBelowMinimum or OperationError.BetAboveMaximum =>
            $"Bets must be between {Money(game.MinBet)} and {Money(game.MaxBet)}.",
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, "Unhandled error."),
    };

    private static string Describe(ParseResult parsed) => parsed.Error switch
    {
        ParseError.Empty => AvailableActions,
        ParseError.UnknownAction => $"Unknown action '{parsed.Token}'. {AvailableActions}",
        ParseError.MissingAmount => $"Please provide an amount, e.g. '{parsed.Token?.ToLowerInvariant()} 10'.",
        ParseError.InvalidAmount => $"'{parsed.Token}' is not a valid amount. Use a number like 10 or 10.50.",
        ParseError.TooManyArguments => $"Too many values. {AvailableActions}",
        _ => throw new ArgumentOutOfRangeException(nameof(parsed), parsed.Error, "Unhandled parse error."),
    };

    private string WithBalance(string message) => $"{message} Your current balance is: {Money(wallet.Balance)}";

    private static string Money(decimal amount) => "$" + amount.ToString("#,0.00", CultureInfo.InvariantCulture);
}
