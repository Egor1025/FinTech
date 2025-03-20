namespace FinTech;

public class Operation
{
    public Guid Id { get; private set; }
    public TransactionType Type { get; private set; }
    public Guid BankAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; set; }
    public Guid CategoryId { get; private set; }

    public Operation(TransactionType type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма должна быть положительной", nameof(amount));
        Id = Guid.NewGuid();
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        CategoryId = categoryId;
    }

    [System.Text.Json.Serialization.JsonConstructor]
    public Operation(Guid id, TransactionType type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId)
        : this(type, bankAccountId, amount, date, description, categoryId)
    {
        Id = id;
    }

    public void Update(decimal newAmount, DateTime newDate, string newDescription, Guid newCategoryId)
    {
        Amount = newAmount;
        Date = newDate;
        Description = newDescription;
        CategoryId = newCategoryId;
    }

    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}