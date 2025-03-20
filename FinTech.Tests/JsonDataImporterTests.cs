namespace FinTech.Tests;

public class JsonDataImporterTests
{
    [Fact]
    public void Import_ValidJson_ImportsData()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        var data = new ImportExportData
        {
            Accounts = new System.Collections.Generic.List<BankAccount>
            {
                DomainFactory.CreateBankAccount("Acc1", 100)
            },
            Categories = new System.Collections.Generic.List<Category>
            {
                DomainFactory.CreateCategory("Cat1", TransactionType.Income)
            },
            Operations = new System.Collections.Generic.List<Operation>
            {
                DomainFactory.CreateOperation(TransactionType.Income, DomainFactory.CreateBankAccount("Acc2", 0), 50, DateTime.Now, "Op1", DomainFactory.CreateCategory("Cat2", TransactionType.Expense))
            }
        };
        var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(tempFile, json);

        var manager = new FinanceManager();
        var importer = new JsonDataImporter();

        // Act
        importer.Import(tempFile, manager);

        // Assert
        Assert.NotEmpty(manager.GetBankAccounts());
        Assert.NotEmpty(manager.GetCategories());
        Assert.NotEmpty(manager.GetOperations());

        File.Delete(tempFile);
    }
}