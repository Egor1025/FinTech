namespace FinTech.Tests;

public class DomainFactoryTests
{
    [Fact]
    public void CreateBankAccount_ReturnsValidAccount()
    {
        var account = DomainFactory.CreateBankAccount("Test", 100);
        Assert.Equal("Test", account.Name);
        Assert.Equal(100, account.Balance);
        Assert.NotEqual(Guid.Empty, account.Id);
    }

    [Fact]
    public void CreateCategory_ReturnsValidCategory()
    {
        var category = DomainFactory.CreateCategory("Food", TransactionType.Expense);
        Assert.Equal("Food", category.Name);
        Assert.Equal(TransactionType.Expense, category.Type);
        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void CreateOperation_Throws_WhenExpenseGreaterThanBalance()
    {
        var account = DomainFactory.CreateBankAccount("Test", 100);
        var category = DomainFactory.CreateCategory("Food", TransactionType.Expense);
        Assert.Throws<InvalidOperationException>(() => DomainFactory.CreateOperation(TransactionType.Expense, account, 150, DateTime.Now, "Test", category));
    }
}