namespace FinTech;

public class FinanceManagerProxy : IFinanceManager
{
    private readonly IFinanceManager _realManager;
    private IEnumerable<BankAccount> _cachedAccounts;
    private DateTime _cacheTimestamp;

    public FinanceManagerProxy(IFinanceManager realManager)
    {
        _realManager = realManager;
    }

    public void AddBankAccount(BankAccount account)
    {
        _realManager.AddBankAccount(account);
        InvalidateCache();
    }

    public void RemoveBankAccount(Guid accountId)
    {
        _realManager.RemoveBankAccount(accountId);
        InvalidateCache();
    }

    public IEnumerable<BankAccount> GetBankAccounts()
    {
        if (_cachedAccounts != null && !((DateTime.Now - _cacheTimestamp).TotalSeconds > 30)) return _cachedAccounts;
        _cachedAccounts = _realManager.GetBankAccounts();
        _cacheTimestamp = DateTime.Now;
        return _cachedAccounts;
    }

    public void AddCategory(Category category) => _realManager.AddCategory(category);
    public void RemoveCategory(Guid categoryId) => _realManager.RemoveCategory(categoryId);
    public IEnumerable<Category> GetCategories() => _realManager.GetCategories();
    public void AddOperation(Operation operation) => _realManager.AddOperation(operation);
    public void RemoveOperation(Guid operationId) => _realManager.RemoveOperation(operationId);
    public IEnumerable<Operation> GetOperations() => _realManager.GetOperations();
    public decimal GetIncomeExpenseDifference(DateTime start, DateTime end) => _realManager.GetIncomeExpenseDifference(start, end);
    public Dictionary<Guid, decimal> GroupOperationsByCategory(DateTime start, DateTime end) => _realManager.GroupOperationsByCategory(start, end);
    public void ExportToCsv(string directory) => _realManager.ExportToCsv(directory);
    public void ExportToJson(string filePath) => _realManager.ExportToJson(filePath);
    public void ImportFromCsv(string directory) => _realManager.ImportFromCsv(directory);
    public void ImportFromJson(string filePath) => _realManager.ImportFromJson(filePath);
    public void ImportFromJsonFromData(ImportExportData data) => _realManager.ImportFromJsonFromData(data);
    public void EditBankAccount(Guid accountId, string newName)
    {
        _realManager.EditBankAccount(accountId, newName);
        InvalidateCache();
    }
    public void EditCategory(Guid categoryId, string newName, TransactionType newType) => _realManager.EditCategory(categoryId, newName, newType);
    public void EditOperation(Guid operationId, decimal newAmount, DateTime newDate, string newDescription, Guid newCategoryId) => _realManager.EditOperation(operationId, newAmount, newDate, newDescription, newCategoryId);
    private void InvalidateCache() => _cachedAccounts = null;
}