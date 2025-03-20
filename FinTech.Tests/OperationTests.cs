namespace FinTech.Tests;

public class OperationTests
{
    [Fact]
    public void CreateOperation_SetsProperties()
    {
        var account = new BankAccount("Test", 100);
        var category = new Category("Salary", TransactionType.Income);
        var op = new Operation(TransactionType.Income, account.Id, 200, DateTime.Now, "Test", category.Id);
        Assert.Equal(TransactionType.Income, op.Type);
        Assert.Equal(account.Id, op.BankAccountId);
        Assert.Equal(200, op.Amount);
        Assert.Equal(category.Id, op.CategoryId);
        Assert.NotEqual(Guid.Empty, op.Id);
    }

    [Fact]
    public void CreateOperation_Throws_WhenAmountNonPositive()
    {
        var account = new BankAccount("Test", 100);
        var category = new Category("Salary", TransactionType.Income);
        Assert.Throws<ArgumentException>(() => new Operation(TransactionType.Income, account.Id, 0, DateTime.Now, "Test", category.Id));
    }
}