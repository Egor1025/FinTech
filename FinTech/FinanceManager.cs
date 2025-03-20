namespace FinTech;

public class FinanceManager : IFinanceManager
{
    private readonly List<BankAccount> _accounts = new();
    private readonly List<Category> _categories = new();
    private readonly List<Operation> _operations = new();

    public void AddBankAccount(BankAccount account)
    {
        _accounts.Add(account);
    }
    
    public void RemoveBankAccount(Guid accountId)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        if (account != null)
            _accounts.Remove(account);
    }

    public IEnumerable<BankAccount> GetBankAccounts()
    {
        return _accounts;
    }

    public void AddCategory(Category category)
    {
        _categories.Add(category);
    }
    
    public void RemoveCategory(Guid categoryId)
    {
        var category = _categories.FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
            _categories.Remove(category);
    }

    public IEnumerable<Category> GetCategories()
    {
        return _categories;
    }

    public void AddOperation(Operation operation)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == operation.BankAccountId);
        if (account == null)
            throw new InvalidOperationException("Счет не найден");

        if (operation.Type == TransactionType.Income)
            account.Deposit(operation.Amount);
        else
            account.Withdraw(operation.Amount);

        _operations.Add(operation);
    }
    
    public void RemoveOperation(Guid operationId)
    {
        var operation = _operations.FirstOrDefault(o => o.Id == operationId);
        if (operation != null)
            _operations.Remove(operation);
    }

    public IEnumerable<Operation> GetOperations()
    {
        return _operations;
    }

    // Подсчет разницы между доходами и расходами за указанный период
    public decimal GetIncomeExpenseDifference(DateTime start, DateTime end)
    {
        var income = _operations.Where(o => o.Date >= start && o.Date <= end && o.Type == TransactionType.Income)
            .Sum(o => o.Amount);
        var expense = _operations.Where(o => o.Date >= start && o.Date <= end && o.Type == TransactionType.Expense)
            .Sum(o => o.Amount);

        return income - expense;
    }

    // Группировка операций по категориям за указанный период
    public Dictionary<Guid, decimal> GroupOperationsByCategory(DateTime start, DateTime end)
    {
        return _operations.Where(o => o.Date >= start && o.Date <= end)
            .GroupBy(o => o.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(o => o.Amount));
    }

    // Экспорт данных в формат CSV
    public void ExportToCsv(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Неверно указан путь к директории", nameof(directory));

        try
        {
            Directory.CreateDirectory(directory);

            // Экспорт счетов
            var accountsCsv = "Id,Name,Balance\n" + string.Join("\n", _accounts.Select(a => $"{a.Id},{a.Name},{a.Balance}"));
            File.WriteAllText(Path.Combine(directory, "accounts.csv"), accountsCsv);

            // Экспорт категорий
            var categoriesCsv = "Id,Type,Name\n" + string.Join("\n", _categories.Select(c => $"{c.Id},{c.Type},{c.Name}"));
            File.WriteAllText(Path.Combine(directory, "categories.csv"), categoriesCsv);

            // Экспорт операций
            var operationsCsv = "Id,Type,BankAccountId,Amount,Date,Description,CategoryId\n" +
                string.Join("\n", _operations.Select(o => $"{o.Id},{o.Type},{o.BankAccountId},{o.Amount},{o.Date:yyyy-MM-dd},{o.Description},{o.CategoryId}"));
            File.WriteAllText(Path.Combine(directory, "operations.csv"), operationsCsv);

            Console.WriteLine("Данные сохранены в: " + Path.GetFullPath(directory));
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при экспорте данных в CSV: " + ex.Message, ex);
        }
    }

    // Экспорт данных в формат JSON
    public void ExportToJson(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Неверно указан путь к файлу", nameof(filePath));

        try
        {
            var data = new
            {
                Accounts = _accounts,
                Categories = _categories,
                Operations = _operations
            };

            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);

            string directory = Path.GetDirectoryName(filePath);
            Console.WriteLine("Данные сохранены в: " + Path.GetFullPath(directory));
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при экспорте данных в JSON: " + ex.Message, ex);
        }
    }
    
    // Реализация редактирования объектов
    public void EditBankAccount(Guid accountId, string newName)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        if (account == null)
            throw new InvalidOperationException("Счет не найден");
        account.Name = newName;
    }

    public void EditCategory(Guid categoryId, string newName, TransactionType newType)
    {
        var category = _categories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null)
            throw new InvalidOperationException("Категория не найдена");
        category.UpdateName(newName);
        category.UpdateType(newType);
    }

    public void EditOperation(Guid operationId, decimal newAmount, DateTime newDate, string newDescription, Guid newCategoryId)
    {
        var operation = _operations.FirstOrDefault(o => o.Id == operationId);
        if (operation == null)
            throw new InvalidOperationException("Операция не найдена");
        var account = _accounts.FirstOrDefault(a => a.Id == operation.BankAccountId);
        if (account == null)
            throw new InvalidOperationException("Счет не найден");
        decimal oldAmount = operation.Amount;
        if (operation.Type == TransactionType.Income)
        {
            decimal diff = newAmount - oldAmount;
            if (diff >= 0)
                account.Deposit(diff);
            else
                account.Withdraw(-diff);
        }
        else
        {
            decimal diff = newAmount - oldAmount;
            if (diff >= 0)
                account.Withdraw(diff);
            else
                account.Deposit(-diff);
        }
        operation.Update(newAmount, newDate, newDescription, newCategoryId);
    }

    // Импорт данных из формата CSV (упрощенный вариант)
    public void ImportFromCsv(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Неверно указана директория", nameof(directory));

        try
        {
            var accountsFile = Path.Combine(directory, "accounts.csv");
            if (File.Exists(accountsFile))
            {
                var lines = File.ReadAllLines(accountsFile).Skip(1);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    var account = DomainFactory.CreateBankAccount(parts[1], decimal.Parse(parts[2]));
                    typeof(BankAccount).GetProperty("Id").SetValue(account, Guid.Parse(parts[0]));
                    _accounts.Add(account);
                }
            }

            var categoriesFile = Path.Combine(directory, "categories.csv");
            if (File.Exists(categoriesFile))
            {
                var lines = File.ReadAllLines(categoriesFile).Skip(1);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    var category = DomainFactory.CreateCategory(parts[2], parts[1] == "Income" ? TransactionType.Income : TransactionType.Expense);
                    typeof(Category).GetProperty("Id").SetValue(category, Guid.Parse(parts[0]));
                    _categories.Add(category);
                }
            }

            var operationsFile = Path.Combine(directory, "operations.csv");
            if (File.Exists(operationsFile))
            {
                var lines = File.ReadAllLines(operationsFile).Skip(1);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    var operation = new Operation(parts[1] == "Income" ? TransactionType.Income : TransactionType.Expense,
                        Guid.Parse(parts[2]),
                        decimal.Parse(parts[3]),
                        DateTime.Parse(parts[4]),
                        parts[5],
                        Guid.Parse(parts[6]));
                    typeof(Operation).GetProperty("Id").SetValue(operation, Guid.Parse(parts[0]));
                    _operations.Add(operation);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при импорте данных из CSV: " + ex.Message, ex);
        }
    }

    // Импорт данных из формата JSON
    public void ImportFromJson(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Неверно указан путь к файлу", nameof(filePath));

        try
        {
            var json = File.ReadAllText(filePath);
            var data = System.Text.Json.JsonSerializer.Deserialize<ImportExportData>(json);
            if (data != null)
                ImportFromJsonFromData(data);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при импорте данных из JSON: " + ex.Message, ex);
        }
    }
    
    public void ImportFromJsonFromData(ImportExportData data)
    {
        _accounts.Clear();
        _accounts.AddRange(data.Accounts);
        _categories.Clear();
        _categories.AddRange(data.Categories);
        _operations.Clear();
        _operations.AddRange(data.Operations);
    }
}

public class ImportExportData
{
    public List<BankAccount> Accounts { get; set; }
    public List<Category> Categories { get; set; }
    public List<Operation> Operations { get; set; }
}