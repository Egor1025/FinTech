namespace FinTech;

public interface ICommand
{
    void Execute();
}

public class CreateBankAccountCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public CreateBankAccountCommand(IFinanceManager manager) => _manager = manager;
    
    public void Execute()
    {
        Console.Write("Введите название счета: ");
        var name = Console.ReadLine();
        Console.Write("Введите начальный баланс: ");
        var balanceInput = Console.ReadLine();
        decimal balance = 0;
        decimal.TryParse(balanceInput, out balance);
        var account = DomainFactory.CreateBankAccount(name, balance);
        _manager.AddBankAccount(account);
        Console.WriteLine("Счет создан успешно с ID: " + account.Id);
    }
}

public class CreateCategoryCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public CreateCategoryCommand(IFinanceManager manager) => _manager = manager;
    
    public void Execute()
    {
        Console.Write("Введите название категории: ");
        var name = Console.ReadLine();
        Console.Write("Введите тип категории (income/expense): ");
        var typeInput = Console.ReadLine();
        var type = typeInput.ToLower() == "income" ? TransactionType.Income : TransactionType.Expense;
        var category = DomainFactory.CreateCategory(name, type);
        _manager.AddCategory(category);
        Console.WriteLine("Категория создана успешно с ID: " + category.Id);
    }
}

public class CreateOperationCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public CreateOperationCommand(IFinanceManager manager) => _manager = manager;
    
    public void Execute()
    {
        Console.Write("Введите тип операции (income/expense): ");
        var typeInput = Console.ReadLine();
        var type = typeInput.ToLower() == "income" ? TransactionType.Income : TransactionType.Expense;
        Console.WriteLine("Доступные счета:");
        foreach (var _account in _manager.GetBankAccounts())
            Console.WriteLine(_account.Id + " - " + _account.Name + " (Баланс: " + _account.Balance + ")");
        Console.Write("Введите ID счета: ");
        var accountIdInput = Console.ReadLine();
        if (!Guid.TryParse(accountIdInput, out Guid accountId))
        {
            Console.WriteLine("Неверный ID счета");
            return;
        }
        var account = _manager.GetBankAccounts().FirstOrDefault(a => a.Id == accountId);
        if (account == null)
        {
            Console.WriteLine("Счет не найден");
            return;
        }
        Console.Write("Введите сумму операции: ");
        var amountInput = Console.ReadLine();
        if (!decimal.TryParse(amountInput, out decimal amount))
        {
            Console.WriteLine("Неверная сумма");
            return;
        }
        Console.Write("Введите дату операции (формат: yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();
        if (!DateTime.TryParse(dateInput, out DateTime date))
        {
            Console.WriteLine("Неверная дата");
            return;
        }
        Console.Write("Введите описание операции (опционально): ");
        var description = Console.ReadLine();
        Console.WriteLine("Доступные категории:");
        foreach (var category in _manager.GetCategories().Where(c => c.Type == type))
            Console.WriteLine(category.Id + " - " + category.Name);
        Console.Write("Введите ID категории: ");
        var categoryIdInput = Console.ReadLine();
        if (!Guid.TryParse(categoryIdInput, out Guid categoryId))
        {
            Console.WriteLine("Неверный ID категории");
            return;
        }
        var categoryObj = _manager.GetCategories().FirstOrDefault(c => c.Id == categoryId);
        if (categoryObj == null)
        {
            Console.WriteLine("Категория не найдена");
            return;
        }
        try
        {
            var operation = DomainFactory.CreateOperation(type, account, amount, date, description, categoryObj);
            _manager.AddOperation(operation);
            Console.WriteLine("Операция создана успешно с ID: " + operation.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при создании операции: " + ex.Message);
        }
    }
}

public class ListDataCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public ListDataCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.WriteLine("Счета:");
        foreach (var _account in _manager.GetBankAccounts())
            Console.WriteLine(_account.Id + " - " + _account.Name + " (Баланс: " + _account.Balance + ")");
        Console.WriteLine("\nКатегории:");
        foreach (var category in _manager.GetCategories())
            Console.WriteLine(category.Id + " - " + category.Name + " (Тип: " + category.Type + ")");
        Console.WriteLine("\nОперации:");
        foreach (var operation in _manager.GetOperations())
            Console.WriteLine(operation.Id + " - " + operation.Type + " (Счет: " + operation.BankAccountId + ", Сумма: " + operation.Amount + ", Дата: " + operation.Date.ToString("yyyy-MM-dd") + ")");
    }
}

public class AnalyticsCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public AnalyticsCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.Write("Введите начальную дату (yyyy-MM-dd): ");
        var startInput = Console.ReadLine();
        if (!DateTime.TryParse(startInput, out DateTime start))
        {
            Console.WriteLine("Неверная дата");
            return;
        }
        Console.Write("Введите конечную дату (yyyy-MM-dd): ");
        var endInput = Console.ReadLine();
        if (!DateTime.TryParse(endInput, out DateTime end))
        {
            Console.WriteLine("Неверная дата");
            return;
        }
        var difference = _manager.GetIncomeExpenseDifference(start, end);
        Console.WriteLine("Разница между доходами и расходами: " + difference);
        var group = _manager.GroupOperationsByCategory(start, end);
        Console.WriteLine("Суммы по категориям:");
        foreach (var kvp in group)
            Console.WriteLine("Категория ID " + kvp.Key + ": " + kvp.Value);
    }
}

public class ImportExportCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public ImportExportCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1. Экспорт данных в CSV");
        Console.WriteLine("2. Экспорт данных в JSON");
        Console.WriteLine("3. Импорт данных из CSV");
        Console.WriteLine("4. Импорт данных из JSON");
        Console.Write("Ваш выбор: ");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Console.Write("Введите директорию для экспорта CSV: ");
                var dir = Console.ReadLine();
                _manager.ExportToCsv(dir);
                break;
            case "2":
                Console.Write("Введите путь для экспорта JSON: ");
                var jsonPath = Console.ReadLine();
                _manager.ExportToJson(jsonPath);
                break;
            case "3":
                Console.Write("Введите директорию для импорта CSV: ");
                var importDir = Console.ReadLine();
                _manager.ImportFromCsv(importDir);
                Console.WriteLine("Импорт из CSV выполнен");
                break;
            case "4":
                Console.Write("Введите путь для импорта JSON: ");
                var importJsonPath = Console.ReadLine();
                _manager.ImportFromJson(importJsonPath);
                Console.WriteLine("Импорт из JSON выполнен");
                break;
            default:
                Console.WriteLine("Неверный выбор");
                break;
        }
    }
}

public class RemoveBankAccountCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public RemoveBankAccountCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.Write("Введите ID счета для удаления: ");
        var input = Console.ReadLine();
        if (Guid.TryParse(input, out Guid id))
        {
            try
            {
                _manager.RemoveBankAccount(id);
                Console.WriteLine("Счет удален.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при удалении счета: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Неверный ID счета.");
        }
    }
}

public class RemoveCategoryCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public RemoveCategoryCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.Write("Введите ID категории для удаления: ");
        var input = Console.ReadLine();
        if (Guid.TryParse(input, out Guid id))
        {
            try
            {
                _manager.RemoveCategory(id);
                Console.WriteLine("Категория удалена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при удалении категории: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Неверный ID категории.");
        }
    }
}

public class RemoveOperationCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public RemoveOperationCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.Write("Введите ID операции для удаления: ");
        var input = Console.ReadLine();
        if (Guid.TryParse(input, out Guid id))
        {
            try
            {
                _manager.RemoveOperation(id);
                Console.WriteLine("Операция удалена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при удалении операции: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Неверный ID операции.");
        }
    }
}

public class EditBankAccountCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public EditBankAccountCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.Write("Введите ID счета для редактирования: ");
        var input = Console.ReadLine();
        if (!Guid.TryParse(input, out Guid id))
        {
            Console.WriteLine("Неверный ID счета.");
            return;
        }
        Console.Write("Введите новое название счета: ");
        var newName = Console.ReadLine();
        try
        {
            _manager.EditBankAccount(id, newName);
            Console.WriteLine("Счет успешно отредактирован.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при редактировании счета: " + ex.Message);
        }
    }
}

public class EditCategoryCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public EditCategoryCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.Write("Введите ID категории для редактирования: ");
        var input = Console.ReadLine();
        if (!Guid.TryParse(input, out Guid id))
        {
            Console.WriteLine("Неверный ID категории.");
            return;
        }
        Console.Write("Введите новое название категории: ");
        var newName = Console.ReadLine();
        Console.Write("Введите новый тип категории (income/expense): ");
        var typeInput = Console.ReadLine();
        var newType = typeInput.ToLower() == "income" ? TransactionType.Income : TransactionType.Expense;
        try
        {
            _manager.EditCategory(id, newName, newType);
            Console.WriteLine("Категория успешно отредактирована.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при редактировании категории: " + ex.Message);
        }
    }
}

public class EditOperationCommand : ICommand
{
    private readonly IFinanceManager _manager;
    public EditOperationCommand(IFinanceManager manager)
    {
        _manager = manager;
    }
    public void Execute()
    {
        Console.Write("Введите ID операции для редактирования: ");
        var input = Console.ReadLine();
        if (!Guid.TryParse(input, out Guid id))
        {
            Console.WriteLine("Неверный ID операции.");
            return;
        }
        Console.Write("Введите новую сумму операции: ");
        var amountInput = Console.ReadLine();
        if (!decimal.TryParse(amountInput, out decimal newAmount))
        {
            Console.WriteLine("Неверная сумма.");
            return;
        }
        Console.Write("Введите новую дату операции (формат: yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();
        if (!DateTime.TryParse(dateInput, out DateTime newDate))
        {
            Console.WriteLine("Неверная дата.");
            return;
        }
        Console.Write("Введите новое описание операции (опционально): ");
        var newDescription = Console.ReadLine();
        Console.Write("Введите новый ID категории: ");
        var catInput = Console.ReadLine();
        if (!Guid.TryParse(catInput, out Guid newCategoryId))
        {
            Console.WriteLine("Неверный ID категории.");
            return;
        }
        try
        {
            _manager.EditOperation(id, newAmount, newDate, newDescription, newCategoryId);
            Console.WriteLine("Операция успешно отредактирована.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при редактировании операции: " + ex.Message);
        }
    }
}