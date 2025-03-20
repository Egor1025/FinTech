namespace FinTech.Tests;

public class FinanceManagerTests
{
    [Fact]
    public void AddAndRetrieveBankAccount()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Test", 100);
        manager.AddBankAccount(account);
        var accounts = manager.GetBankAccounts().ToList();
        Assert.Single(accounts);
        Assert.Equal(account.Id, accounts[0].Id);
    }

    [Fact]
    public void GetIncomeExpenseDifference_CalculatesCorrectly()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Test", 100);
        manager.AddBankAccount(account);
        var categoryIncome = DomainFactory.CreateCategory("Salary", TransactionType.Income);
        var categoryExpense = DomainFactory.CreateCategory("Food", TransactionType.Expense);
        manager.AddCategory(categoryIncome);
        manager.AddCategory(categoryExpense);
        var now = DateTime.Now;
        var op1 = DomainFactory.CreateOperation(TransactionType.Income, account, 200, now, "Income", categoryIncome);
        manager.AddOperation(op1);
        var op2 = new Operation(TransactionType.Expense, account.Id, 50, now, "Expense", categoryExpense.Id);
        manager.AddOperation(op2);
        var diff = manager.GetIncomeExpenseDifference(now.AddMinutes(-1), now.AddMinutes(1));
        Assert.Equal(150, diff);
    }

    [Fact]
    public void GroupOperationsByCategory_ReturnsCorrectGrouping()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Test", 100);
        manager.AddBankAccount(account);
        var category1 = DomainFactory.CreateCategory("Salary", TransactionType.Income);
        var category2 = DomainFactory.CreateCategory("Food", TransactionType.Expense);
        manager.AddCategory(category1);
        manager.AddCategory(category2);
        var now = DateTime.Now;
        var op1 = DomainFactory.CreateOperation(TransactionType.Income, account, 200, now, "Income", category1);
        var op2 = new Operation(TransactionType.Expense, account.Id, 50, now, "Expense", category2.Id);
        manager.AddOperation(op1);
        manager.AddOperation(op2);
        var groups = manager.GroupOperationsByCategory(now.AddMinutes(-1), now.AddMinutes(1));
        Assert.Equal(2, groups.Count);
        Assert.Equal(200, groups[category1.Id]);
        Assert.Equal(50, groups[category2.Id]);
    }

    [Fact]
    public void ExportAndImport_CsvAndJson_WorksCorrectly()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Test", 100);
        manager.AddBankAccount(account);
        var category = DomainFactory.CreateCategory("Food", TransactionType.Expense);
        manager.AddCategory(category);
        var op = new Operation(TransactionType.Expense, account.Id, 50, DateTime.Now, "Test", category.Id);
        manager.AddOperation(op);
        
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            manager.ExportToCsv(tempDir);
            var jsonPath = Path.Combine(tempDir, "data.json");
            manager.ExportToJson(jsonPath);
            
            var manager2 = new FinanceManager();
            manager2.ImportFromCsv(tempDir);
            manager2.ImportFromJson(jsonPath);
            
            Assert.Single(manager2.GetBankAccounts());
            Assert.Single(manager2.GetCategories());
            Assert.Single(manager2.GetOperations());
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}