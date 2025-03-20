namespace FinTech.Tests;

public class BankAccountTests
{
    [Fact]
    public void Deposit_IncreasesBalance()
    {
        var account = new BankAccount("Test", 100);
        account.Deposit(50);
        Assert.Equal(150, account.Balance);
    }

    [Fact]
    public void Withdraw_DecreasesBalance()
    {
        var account = new BankAccount("Test", 100);
        account.Withdraw(30);
        Assert.Equal(70, account.Balance);
    }

    [Fact]
    public void Withdraw_Throws_WhenInsufficientFunds()
    {
        var account = new BankAccount("Test", 100);
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(150));
    }

    [Fact]
    public void Deposit_Throws_WhenNegativeAmount()
    {
        var account = new BankAccount("Test", 100);
        Assert.Throws<ArgumentException>(() => account.Deposit(-10));
    }
}