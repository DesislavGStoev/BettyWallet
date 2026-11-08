using BettyWallet.Core;

namespace BettyWallet.Tests.Core;

public class WalletTests
{
    [Fact]
    public void Deposit_increases_balance()
    {
        var wallet = TestFactory.NewWallet();

        var result = wallet.Deposit(10.50m);

        Assert.True(result.IsSuccess);
        Assert.Equal(10.50m, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Deposit_rejects_non_positive_amount(decimal amount)
    {
        var wallet = TestFactory.NewWallet();

        var result = wallet.Deposit(amount);

        Assert.Equal(OperationError.NonPositiveAmount, result.Error);
        Assert.Equal(0m, wallet.Balance);
    }

    [Fact]
    public void Deposit_rejects_fractions_of_a_cent()
    {
        var wallet = TestFactory.NewWallet();

        var result = wallet.Deposit(10.555m);

        Assert.Equal(OperationError.TooManyDecimals, result.Error);
        Assert.Equal(0m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_decreases_balance()
    {
        var wallet = WalletWith(44.15m);

        var result = wallet.Withdraw(44.00m);

        Assert.True(result.IsSuccess);
        Assert.Equal(0.15m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_more_than_balance_is_rejected()
    {
        var wallet = WalletWith(5m);

        var result = wallet.Withdraw(5.01m);

        Assert.Equal(OperationError.InsufficientFunds, result.Error);
        Assert.Equal(5m, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Withdraw_rejects_non_positive_amount(decimal amount)
    {
        var wallet = WalletWith(5m);

        var result = wallet.Withdraw(amount);

        Assert.Equal(OperationError.NonPositiveAmount, result.Error);
        Assert.Equal(5m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_rejects_fractions_of_a_cent()
    {
        var wallet = WalletWith(5m);

        var result = wallet.Withdraw(1.001m);

        Assert.Equal(OperationError.TooManyDecimals, result.Error);
        Assert.Equal(5m, wallet.Balance);
    }

    [Fact]
    public void Debit_bet_decreases_balance()
    {
        var wallet = WalletWith(10m);

        var result = wallet.DebitBet(5m);

        Assert.True(result.IsSuccess);
        Assert.Equal(5m, wallet.Balance);
    }

    [Fact]
    public void Debit_bet_larger_than_balance_is_rejected()
    {
        var wallet = WalletWith(4.99m);

        var result = wallet.DebitBet(5m);

        Assert.Equal(OperationError.InsufficientFunds, result.Error);
        Assert.Equal(4.99m, wallet.Balance);
    }

    [Fact]
    public void Debit_bet_rejects_fractions_of_a_cent()
    {
        var wallet = WalletWith(10m);

        var result = wallet.DebitBet(1.005m);

        Assert.Equal(OperationError.TooManyDecimals, result.Error);
        Assert.Equal(10m, wallet.Balance);
    }

    [Fact]
    public void Debit_then_credit_gives_old_minus_bet_plus_win()
    {
        var wallet = WalletWith(10m);

        wallet.DebitBet(5m);
        wallet.CreditWin(35.35m);

        Assert.Equal(40.35m, wallet.Balance);
    }

    [Fact]
    public void Credit_win_of_zero_leaves_balance_unchanged()
    {
        var wallet = WalletWith(10m);

        wallet.CreditWin(0m);

        Assert.Equal(10m, wallet.Balance);
    }

    private static Wallet WalletWith(decimal balance)
    {
        var wallet = TestFactory.NewWallet();
        wallet.Deposit(balance);
        return wallet;
    }
}
