namespace FinTech.Tests;

public class InvalidExportImportTests
{
    [Fact]
    public void ExportToCsv_ThrowsException_OnInvalidDirectory()
    {
        var manager = new FinanceManager();
        Assert.Throws<ArgumentException>(() => manager.ExportToCsv(""));
    }

    [Fact]
    public void ExportToJson_ThrowsException_OnInvalidFilePath()
    {
        var manager = new FinanceManager();
        Assert.Throws<ArgumentException>(() => manager.ExportToJson(" "));
    }

    [Fact]
    public void ImportFromCsv_ThrowsException_OnInvalidDirectory()
    {
        var manager = new FinanceManager();
        Assert.Throws<ArgumentException>(() => manager.ImportFromCsv(""));
    }

    [Fact]
    public void ImportFromJson_ThrowsException_OnInvalidFilePath()
    {
        var manager = new FinanceManager();
        Assert.Throws<ArgumentException>(() => manager.ImportFromJson(" "));
    }
}