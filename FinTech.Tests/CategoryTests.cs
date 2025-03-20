namespace FinTech.Tests;

public class CategoryTests
{
    [Fact]
    public void CreateCategory_SetsProperties()
    {
        var category = new Category("Food", TransactionType.Expense);
        Assert.Equal("Food", category.Name);
        Assert.Equal(TransactionType.Expense, category.Type);
        Assert.NotEqual(Guid.Empty, category.Id);
    }
}