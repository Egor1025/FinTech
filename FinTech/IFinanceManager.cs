namespace FinTech;

public interface IFinanceManager
{
    void AddBankAccount(BankAccount account);
    void RemoveBankAccount(Guid accountId);
    IEnumerable<BankAccount> GetBankAccounts();
    void AddCategory(Category category);
    void RemoveCategory(Guid categoryId);
    IEnumerable<Category> GetCategories();
    void AddOperation(Operation operation);
    void RemoveOperation(Guid operationId);
    IEnumerable<Operation> GetOperations();
    decimal GetIncomeExpenseDifference(DateTime start, DateTime end);
    Dictionary<Guid, decimal> GroupOperationsByCategory(DateTime start, DateTime end);
    void ExportToCsv(string directory);
    void ExportToJson(string filePath);
    void ImportFromCsv(string directory);
    void ImportFromJson(string filePath);
    void ImportFromJsonFromData(ImportExportData data);
    void EditBankAccount(Guid accountId, string newName);
    void EditCategory(Guid categoryId, string newName, TransactionType newType);
    void EditOperation(Guid operationId, decimal newAmount, DateTime newDate, string newDescription, Guid newCategoryId);

}