namespace FinTech;

public class CsvExportVisitor : IExportVisitor
{
    private readonly List<string> _lines = new();

    public void Visit(BankAccount account)
    {
        _lines.Add($"{account.Id},{account.Name},{account.Balance}");
    }

    public void Visit(Category category)
    {
        _lines.Add($"{category.Id},{category.Type},{category.Name}");
    }

    public void Visit(Operation operation)
    {
        _lines.Add($"{operation.Id},{operation.Type},{operation.BankAccountId},{operation.Amount},{operation.Date:yyyy-MM-dd},{operation.Description},{operation.CategoryId}");
    }

    public string GetResult() => string.Join("\n", _lines);
}