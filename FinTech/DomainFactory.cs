namespace FinTech;

public static class DomainFactory
{
    // Создание счета
    public static BankAccount CreateBankAccount(string name, decimal initialBalance = 0)
    {
        return new BankAccount(name, initialBalance);
    }

    // Создание категории
    public static Category CreateCategory(string name, TransactionType type)
    {
        return new Category(name, type);
    }

    // Создание операции с проверкой баланса для расхода
    public static Operation CreateOperation(TransactionType type, BankAccount account, decimal amount, DateTime date, string description, Category category)
    {
        if (type == TransactionType.Expense && amount > account.Balance)
            throw new InvalidOperationException("Недостаточно средств на счете для проведения операции расхода");
        
        return new Operation(type, account.Id, amount, date, description, category.Id);
    }
}