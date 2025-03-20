namespace FinTech.Tests;

public class CsvExportVisitorTests
{
    [Fact]
    public void CsvExportVisitor_CollectsDataCorrectly()
    {
        // Arrange
        var visitor = new CsvExportVisitor();
        var account = DomainFactory.CreateBankAccount("Acc1", 100);
        var category = DomainFactory.CreateCategory("Cat1", TransactionType.Income);
        var operation = DomainFactory.CreateOperation(TransactionType.Income, account, 150, DateTime.Now, "Op1", category);
        
        // Act
        account.Accept(visitor);
        category.Accept(visitor);
        operation.Accept(visitor);
        var result = visitor.GetResult();

        // Assert: проверка наличия идентификаторов объектов в результатах экспорта
        Assert.Contains(account.Id.ToString(), result);
        Assert.Contains(category.Id.ToString(), result);
        Assert.Contains(operation.Id.ToString(), result);
    }
}