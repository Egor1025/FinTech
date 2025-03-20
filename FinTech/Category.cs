namespace FinTech;

public enum TransactionType { Income, Expense }

public class Category
{
    public Guid Id { get; private set; }
    public TransactionType Type { get; private set; }
    public string Name { get; set; }

    public Category(string name, TransactionType type)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
    }

    [System.Text.Json.Serialization.JsonConstructor]
    public Category(Guid id, TransactionType type, string name) : this(name, type)
    {
        Id = id;
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public void UpdateType(TransactionType newType)
    {
        Type = newType;
    }

    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}