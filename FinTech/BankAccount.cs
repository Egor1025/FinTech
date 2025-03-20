namespace FinTech;

public class BankAccount
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public decimal Balance { get; private set; }

    public BankAccount(string name, decimal initialBalance = 0)
    {
        Id = Guid.NewGuid();
        Name = name;
        Balance = initialBalance;
    }

    [System.Text.Json.Serialization.JsonConstructor]
    public BankAccount(Guid id, string name, decimal balance) : this(name, balance)
    {
        Id = id;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма должна быть положительной", nameof(amount));
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма должна быть положительной", nameof(amount));
        if (amount > Balance)
            throw new InvalidOperationException("Недостаточно средств на счете");
        Balance -= amount;
    }
    
    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}