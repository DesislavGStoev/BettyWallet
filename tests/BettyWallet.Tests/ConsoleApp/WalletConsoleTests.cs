using BettyWallet.ConsoleApp;
using BettyWallet.Core.Game;
using BettyWallet.Tests.Core;

namespace BettyWallet.Tests.ConsoleApp;

public class WalletConsoleTests
{
    private const string Prompt = "Please, submit action:";

    [Fact]
    public void Prompts_for_an_action()
    {
        var output = Run(["exit"]);

        Assert.StartsWith(Prompt, output);
    }

    [Fact]
    public void Deposit_shows_amount_and_new_balance()
    {
        var output = Run(["deposit 10", "exit"]);

        Assert.Contains("Your deposit of $10.00 was successful. Your current balance is: $10.00", output);
    }

    [Fact]
    public void Withdrawal_shows_amount_and_new_balance()
    {
        var output = Run(["deposit 44.15", "withdraw 44", "exit"]);

        Assert.Contains("Your withdrawal of $44.00 was successful. Your current balance is: $0.15", output);
    }

    [Fact]
    public void Winning_bet_shows_win_and_new_balance()
    {
        // big win with multiplier 2 + 8 * 0.75875 = x8.07 -> $5 * 8.07 = $40.35
        var output = Run(["deposit 10", "bet 5", "exit"], rolls: [0.95m, 0.75875m]);

        Assert.Contains("Congrats - you won $40.35! Your current balance is: $45.35", output);
    }

    [Fact]
    public void Losing_bet_shows_new_balance()
    {
        var output = Run(["deposit 54.15", "bet 10", "exit"], rolls: [0.1m, 0.5m]);

        Assert.Contains("No luck this time! Your current balance is: $44.15", output);
    }

    [Fact]
    public void Insufficient_funds_shows_unchanged_balance()
    {
        var output = Run(["deposit 3", "withdraw 5", "exit"]);

        Assert.Contains("Insufficient funds. Your current balance is: $3.00", output);
    }

    [Fact]
    public void Bet_outside_limits_shows_the_limits()
    {
        var output = Run(["deposit 50", "bet 11", "exit"]);

        Assert.Contains("Bets must be between $1.00 and $10.00. Your current balance is: $50.00", output);
    }

    [Fact]
    public void Non_positive_amount_is_rejected()
    {
        var output = Run(["deposit -5", "exit"]);

        Assert.Contains("The amount must be a positive number. Your current balance is: $0.00", output);
    }

    [Fact]
    public void Fractions_of_a_cent_are_rejected()
    {
        var output = Run(["deposit 10.555", "exit"]);

        Assert.Contains("The amount can have at most 2 decimal places. Your current balance is: $0.00", output);
    }

    [Fact]
    public void Unknown_action_lists_available_actions()
    {
        var output = Run(["spin 5", "exit"]);

        Assert.Contains("Unknown action 'spin'.", output);
        Assert.Contains("deposit <amount>, withdraw <amount>, bet <amount>, exit", output);
    }

    [Fact]
    public void Invalid_amount_is_reported()
    {
        var output = Run(["deposit abc", "exit"]);

        Assert.Contains("'abc' is not a valid amount.", output);
    }

    /// <summary>Plays a whole session with the given input lines and returns everything the console printed.</summary>
    private static string Run(string[] inputLines, decimal[]? rolls = null)
    {
        var wallet = TestFactory.NewWallet();
        var game = new SlotGame(TestSettings.Default(), new FakeRandomSource(rolls ?? []));
        var bettingService = TestFactory.NewBettingService(wallet, game);

        var input = new StringReader(string.Join(Environment.NewLine, inputLines));
        var output = new StringWriter();

        new WalletConsole(wallet, bettingService, game, input, output).Run();

        return output.ToString();
    }
}
