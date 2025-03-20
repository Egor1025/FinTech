namespace FinTech.Tests;

public class CommandsTests
{
    [Fact]
    public void RemoveBankAccountCommand_RemovesExistingAccount()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("TestAccount", 100);
        manager.AddBankAccount(account);

        var input = new StringReader(account.Id.ToString());
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var command = new RemoveBankAccountCommand(manager);
        command.Execute();

        var result = output.ToString();
        Assert.Contains("Счет удален", result);
        Assert.Empty(manager.GetBankAccounts());
    }

    [Fact]
    public void RemoveCategoryCommand_InvalidInput_ShowsError()
    {
        var manager = new FinanceManager();

        var input = new StringReader("invalid-guid");
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var command = new RemoveCategoryCommand(manager);
        command.Execute();

        var result = output.ToString();
        Assert.Contains("Неверный ID категории", result);
    }

    [Fact]
    public void RemoveOperationCommand_RemovesExistingOperation()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("TestAccount", 200);
        manager.AddBankAccount(account);
        var category = DomainFactory.CreateCategory("TestCategory", TransactionType.Expense);
        manager.AddCategory(category);
        var operation = DomainFactory.CreateOperation(TransactionType.Expense, account, 50, DateTime.Now, "TestOp", category);
        manager.AddOperation(operation);

        var input = new StringReader(operation.Id.ToString());
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var command = new RemoveOperationCommand(manager);
        command.Execute();

        var result = output.ToString();
        Assert.Contains("Операция удалена", result);
        Assert.Empty(manager.GetOperations());
    }
}


public class EditCommandsTests
{
    [Fact]
    public void EditBankAccountCommand_UpdatesAccountSuccessfully()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("OldAccount", 100);
        manager.AddBankAccount(account);

        // Симуляция ввода: сначала ID счета, затем новое имя.
        var input = new StringReader(account.Id.ToString() + Environment.NewLine + "NewAccountName" + Environment.NewLine);
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new EditBankAccountCommand(manager);
        cmd.Execute();

        var updatedAccount = manager.GetBankAccounts().First(a => a.Id == account.Id);
        Assert.Equal("NewAccountName", updatedAccount.Name);
        Assert.Contains("Счет успешно отредактирован", output.ToString());
    }

    [Fact]
    public void EditCategoryCommand_UpdatesCategorySuccessfully()
    {
        var manager = new FinanceManager();
        var category = DomainFactory.CreateCategory("OldCategory", TransactionType.Income);
        manager.AddCategory(category);

        // Симуляция ввода: ID категории, новое имя, новый тип (expense).
        var input = new StringReader(category.Id.ToString() + Environment.NewLine + "NewCategory" + Environment.NewLine + "expense" + Environment.NewLine);
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new EditCategoryCommand(manager);
        cmd.Execute();

        var updatedCategory = manager.GetCategories().First(c => c.Id == category.Id);
        Assert.Equal("NewCategory", updatedCategory.Name);
        Assert.Equal(TransactionType.Expense, updatedCategory.Type);
        Assert.Contains("Категория успешно отредактирована", output.ToString());
    }

    [Fact]
    public void EditOperationCommand_UpdatesOperationSuccessfully()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("TestAccount", 1000);
        manager.AddBankAccount(account);
        var category1 = DomainFactory.CreateCategory("Cat1", TransactionType.Expense);
        var category2 = DomainFactory.CreateCategory("Cat2", TransactionType.Expense);
        manager.AddCategory(category1);
        manager.AddCategory(category2);

        // Создаем операцию расхода 200, баланс должен стать 800.
        var op = DomainFactory.CreateOperation(TransactionType.Expense, account, 200, DateTime.Now, "Initial", category1);
        manager.AddOperation(op);
        Assert.Equal(800, account.Balance);

        // Симуляция ввода: ID операции, новая сумма (150), новая дата, новое описание, новый ID категории.
        var newDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        var inputLines = op.Id.ToString() + Environment.NewLine +
                         "150" + Environment.NewLine +
                         newDate + Environment.NewLine +
                         "Updated" + Environment.NewLine +
                         category2.Id.ToString() + Environment.NewLine;
        var input = new StringReader(inputLines);
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new EditOperationCommand(manager);
        cmd.Execute();

        var updatedOp = manager.GetOperations().First(o => o.Id == op.Id);
        Assert.Equal(150, updatedOp.Amount);
        Assert.Equal("Updated", updatedOp.Description);
        Assert.Equal(category2.Id, updatedOp.CategoryId);
        // При уменьшении расхода с 200 до 150 баланс должен увеличиться на 50 (800 + 50 = 850).
        Assert.Equal(850, account.Balance);
        Assert.Contains("Операция успешно отредактирована", output.ToString());
    }
}


public class CommandsCoverageTests
{
    [Fact]
    public void CreateBankAccountCommand_ValidInput_CreatesAccount()
    {
        var manager = new FinanceManager();
        
        // Симуляция ввода: имя счета + баланс
        var input = new StringReader("Test Account\n100\n");
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new CreateBankAccountCommand(manager);
        cmd.Execute();

        var result = output.ToString();
        Assert.Contains("Счет создан успешно", result);
        Assert.Single(manager.GetBankAccounts());
        Assert.Equal("Test Account", manager.GetBankAccounts().First().Name);
        Assert.Equal(100, manager.GetBankAccounts().First().Balance);
    }

    [Fact]
    public void CreateCategoryCommand_ValidInput_CreatesCategory()
    {
        var manager = new FinanceManager();

        // Симуляция ввода: имя категории + тип (income)
        var input = new StringReader("MyCategory\nincome\n");
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new CreateCategoryCommand(manager);
        cmd.Execute();

        var result = output.ToString();
        Assert.Contains("Категория создана успешно", result);
        Assert.Single(manager.GetCategories());
        Assert.Equal("MyCategory", manager.GetCategories().First().Name);
        Assert.Equal(TransactionType.Income, manager.GetCategories().First().Type);
    }

    [Fact]
    public void CreateOperationCommand_ValidInput_CreatesOperation()
    {
        var manager = new FinanceManager();
        
        // Создаем тестовый счет
        var account = DomainFactory.CreateBankAccount("TestAccount", 50);
        manager.AddBankAccount(account);
        // Создаем категорию
        var category = DomainFactory.CreateCategory("Food", TransactionType.Expense);
        manager.AddCategory(category);

        var inputString = 
            "expense\n" + 
            account.Id + "\n" + 
            "30\n" + 
            "2025-03-20\n" + 
            "Some description\n" + 
            category.Id + "\n";

        var input = new StringReader(inputString);
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new CreateOperationCommand(manager);
        cmd.Execute();

        var result = output.ToString();
        Assert.Contains("Операция создана успешно", result);
        Assert.Single(manager.GetOperations());
        Assert.Equal(20, account.Balance); // 50 - 30 = 20
        Assert.Equal(category.Id, manager.GetOperations().First().CategoryId);
    }

    [Fact]
    public void ListDataCommand_ShowsData()
    {
        var manager = new FinanceManager();

        // Добавим тестовые объекты
        var account = DomainFactory.CreateBankAccount("Acc", 100);
        manager.AddBankAccount(account);
        var category = DomainFactory.CreateCategory("Cat", TransactionType.Expense);
        manager.AddCategory(category);
        var operation = DomainFactory.CreateOperation(TransactionType.Expense, account, 30, DateTime.Now, "Desc", category);
        manager.AddOperation(operation);

        var input = new StringReader("");
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new ListDataCommand(manager);
        cmd.Execute();

        var result = output.ToString();
        // Проверяем, что в выводе содержатся данные
        Assert.Contains(account.Id.ToString(), result);
        Assert.Contains(category.Id.ToString(), result);
        Assert.Contains(operation.Id.ToString(), result);
    }

    [Fact]
    public void AnalyticsCommand_ShowsDifferenceAndGroups()
    {
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Acc", 1000);
        manager.AddBankAccount(account);
        var catIncome = DomainFactory.CreateCategory("IncomeCat", TransactionType.Income);
        var catExpense = DomainFactory.CreateCategory("ExpenseCat", TransactionType.Expense);
        manager.AddCategory(catIncome);
        manager.AddCategory(catExpense);

        // Добавим операции
        manager.AddOperation(DomainFactory.CreateOperation(TransactionType.Income, account, 200, DateTime.Now, "IncomeOp", catIncome));
        manager.AddOperation(DomainFactory.CreateOperation(TransactionType.Expense, account, 50, DateTime.Now, "ExpenseOp", catExpense));

        // Симуляция ввода: начальная и конечная даты
        var inputString = 
            DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "\n" + 
            DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "\n";
        var input = new StringReader(inputString);
        Console.SetIn(input);

        var output = new StringWriter();
        Console.SetOut(output);

        var cmd = new AnalyticsCommand(manager);
        cmd.Execute();

        var result = output.ToString();
        Assert.Contains("Разница между доходами и расходами", result);
        Assert.Contains("Суммы по категориям:", result);
        // Проверяем, что разница должна быть 150
        Assert.Contains("150", result);
    }
}