using BettyWallet.ConsoleApp.Commands;

namespace BettyWallet.Tests.ConsoleApp;

public class CommandParserTests
{
    [Theory]
    [InlineData("deposit 10", ActionType.Deposit, 10)]
    [InlineData("withdraw 44.00", ActionType.Withdraw, 44)]
    [InlineData("bet 5.5", ActionType.Bet, 5.5)]
    [InlineData("BET 5", ActionType.Bet, 5)]
    [InlineData("  deposit   10.50  ", ActionType.Deposit, 10.5)]
    [InlineData("deposit .5", ActionType.Deposit, 0.5)]
    public void Parses_actions_with_an_amount(string input, ActionType expectedType, decimal expectedAmount)
    {
        var result = CommandParser.Parse(input);

        Assert.Equal(ParseError.None, result.Error);
        Assert.Equal(new PlayerAction(expectedType, expectedAmount), result.Action);
    }

    [Theory]
    [InlineData("exit")]
    [InlineData("Exit")]
    public void Parses_exit(string input)
    {
        var result = CommandParser.Parse(input);

        Assert.Equal(ParseError.None, result.Error);
        Assert.Equal(ActionType.Exit, result.Action!.Type);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Blank_input_is_an_error(string? input)
    {
        Assert.Equal(ParseError.Empty, CommandParser.Parse(input).Error);
    }

    [Fact]
    public void Unknown_action_is_an_error()
    {
        var result = CommandParser.Parse("spin 5");

        Assert.Equal(ParseError.UnknownAction, result.Error);
        Assert.Equal("spin", result.Token);
    }

    [Theory]
    [InlineData("deposit")]
    [InlineData("bet")]
    public void Missing_amount_is_an_error(string input)
    {
        Assert.Equal(ParseError.MissingAmount, CommandParser.Parse(input).Error);
    }

    [Theory]
    [InlineData("deposit abc", "abc")]
    [InlineData("deposit 1,000", "1,000")]
    [InlineData("deposit 1e3", "1e3")]
    [InlineData("deposit $10", "$10")]
    public void Non_numeric_amount_is_an_error(string input, string expectedToken)
    {
        var result = CommandParser.Parse(input);

        Assert.Equal(ParseError.InvalidAmount, result.Error);
        Assert.Equal(expectedToken, result.Token);
    }

    [Theory]
    [InlineData("deposit 10 20")]
    [InlineData("exit now")]
    public void Extra_arguments_are_an_error(string input)
    {
        Assert.Equal(ParseError.TooManyArguments, CommandParser.Parse(input).Error);
    }
}
