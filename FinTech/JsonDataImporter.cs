namespace FinTech;

public abstract class DataImporter
{
    public void Import(string path, FinanceManager manager)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Неверно указан путь", nameof(path));
        try
        {
            var content = File.ReadAllText(path);
            ParseData(content, manager);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при импорте данных: " + ex.Message, ex);
        }
    }

    protected abstract void ParseData(string content, FinanceManager manager);
}

public class JsonDataImporter : DataImporter
{
    protected override void ParseData(string content, FinanceManager manager)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<ImportExportData>(content);
        if (data != null)
        {
            manager.ImportFromJsonFromData(data);
        }
    }
}