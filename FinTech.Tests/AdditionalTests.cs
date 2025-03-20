namespace FinTech.Tests;

public class AdditionalFinanceManagerTests
{
    [Fact]
    public void GetIncomeExpenseDifference_ReturnsZero_WhenNoOperations()
    {
        var manager = new FinanceManager();
        var diff = manager.GetIncomeExpenseDifference(DateTime.Now.AddDays(-1), DateTime.Now);
        Assert.Equal(0, diff);
    }

    [Fact]
    public void GroupOperationsByCategory_ReturnsEmptyDictionary_WhenNoOperations()
    {
        var manager = new FinanceManager();
        var groups = manager.GroupOperationsByCategory(DateTime.Now.AddDays(-1), DateTime.Now);
        Assert.Empty(groups);
    }

    [Fact]
    public void GroupOperationsByCategory_SumsMultipleOperationsInSameCategory()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Test", 1000);
        manager.AddBankAccount(account);
        var category = DomainFactory.CreateCategory("Salary", TransactionType.Income);
        manager.AddCategory(category);
        
        // Добавляем две операции дохода в одну и ту же категорию.
        var op1 = DomainFactory.CreateOperation(TransactionType.Income, account, 300, DateTime.Now, "Op1", category);
        var op2 = DomainFactory.CreateOperation(TransactionType.Income, account, 200, DateTime.Now, "Op2", category);
        manager.AddOperation(op1);
        manager.AddOperation(op2);
        
        var groups = manager.GroupOperationsByCategory(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
        Assert.Single(groups);
        Assert.Equal(500, groups[category.Id]);
    }

    [Fact]
    public void RemoveNonExistentBankAccount_DoesNotThrow()
    {
        var manager = new FinanceManager();
        var ex = Record.Exception(() => manager.RemoveBankAccount(Guid.NewGuid()));
        Assert.Null(ex);
    }

    [Fact]
    public void RemoveNonExistentCategory_DoesNotThrow()
    {
        var manager = new FinanceManager();
        var ex = Record.Exception(() => manager.RemoveCategory(Guid.NewGuid()));
        Assert.Null(ex);
    }

    [Fact]
    public void RemoveNonExistentOperation_DoesNotThrow()
    {
        var manager = new FinanceManager();
        var ex = Record.Exception(() => manager.RemoveOperation(Guid.NewGuid()));
        Assert.Null(ex);
    }

    [Fact]
    public void JsonDeserialization_UsesJsonConstructorForBankAccount()
    {
        // Создаем JSON-строку для BankAccount.
        var accountId = Guid.NewGuid();
        var accountJson = "{\"Id\":\"" + accountId + "\", \"Name\":\"TestAccount\", \"Balance\":150}";
        var account = System.Text.Json.JsonSerializer.Deserialize<BankAccount>(accountJson);
        Assert.NotNull(account);
        Assert.Equal("TestAccount", account.Name);
        Assert.Equal(150, account.Balance);
        Assert.Equal(accountId, account.Id);
    }
}