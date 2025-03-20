namespace FinTech.Tests;

public class EditTests
{
    [Fact]
    public void EditBankAccount_UpdatesNameSuccessfully()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("OldName", 100);
        manager.AddBankAccount(account);
        
        // Редактирование имени счета
        manager.EditBankAccount(account.Id, "NewName");
        var updatedAccount = manager.GetBankAccounts().First(a => a.Id == account.Id);
        Assert.Equal("NewName", updatedAccount.Name);
    }

    [Fact]
    public void EditCategory_UpdatesNameAndTypeSuccessfully()
    {
        var manager = new FinanceManager();
        var category = DomainFactory.CreateCategory("OldCategory", TransactionType.Income);
        manager.AddCategory(category);
        
        // Редактирование имени и типа категории
        manager.EditCategory(category.Id, "NewCategory", TransactionType.Expense);
        var updatedCategory = manager.GetCategories().First(c => c.Id == category.Id);
        Assert.Equal("NewCategory", updatedCategory.Name);
        Assert.Equal(TransactionType.Expense, updatedCategory.Type);
    }

    [Fact]
    public void EditOperation_UpdatesPropertiesAndAdjustsBalance()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Account", 1000);
        manager.AddBankAccount(account);
        var category1 = DomainFactory.CreateCategory("Cat1", TransactionType.Expense);
        var category2 = DomainFactory.CreateCategory("Cat2", TransactionType.Expense);
        manager.AddCategory(category1);
        manager.AddCategory(category2);
        
        // Создаем операцию расхода на 200, баланс должен стать 800
        var op = DomainFactory.CreateOperation(TransactionType.Expense, account, 200, DateTime.Now, "Initial", category1);
        manager.AddOperation(op);
        Assert.Equal(800, account.Balance);

        // Редактируем операцию: уменьшаем сумму до 150, меняем описание и категорию
        manager.EditOperation(op.Id, 150, DateTime.Now.AddDays(1), "Updated", category2.Id);
        var updatedOp = manager.GetOperations().First(o => o.Id == op.Id);
        Assert.Equal(150, updatedOp.Amount);
        Assert.Equal("Updated", updatedOp.Description);
        Assert.Equal(category2.Id, updatedOp.CategoryId);
        
        // При уменьшении расхода с 200 до 150 баланс должен увеличиться на 50 (800 + 50 = 850)
        Assert.Equal(850, account.Balance);
    }
}