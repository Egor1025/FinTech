namespace FinTech.Tests;

public class RemovalTests
{
    [Fact]
    public void RemoveBankAccount_RemovesAccountSuccessfully()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("TestAccount", 100);
        manager.AddBankAccount(account);
        Assert.Single(manager.GetBankAccounts());
        
        manager.RemoveBankAccount(account.Id);
        Assert.Empty(manager.GetBankAccounts());
    }

    [Fact]
    public void RemoveCategory_RemovesCategorySuccessfully()
    {
        var manager = new FinanceManager();
        var category = DomainFactory.CreateCategory("TestCategory", TransactionType.Income);
        manager.AddCategory(category);
        Assert.Single(manager.GetCategories());
        
        manager.RemoveCategory(category.Id);
        Assert.Empty(manager.GetCategories());
    }

    [Fact]
    public void RemoveOperation_RemovesOperationSuccessfully()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("TestAccount", 200);
        manager.AddBankAccount(account);
        var category = DomainFactory.CreateCategory("TestCategory", TransactionType.Expense);
        manager.AddCategory(category);
        var operation = DomainFactory.CreateOperation(TransactionType.Expense, account, 50, DateTime.Now, "TestOperation", category);
        manager.AddOperation(operation);
        Assert.Single(manager.GetOperations());
        
        manager.RemoveOperation(operation.Id);
        Assert.Empty(manager.GetOperations());
    }
}