namespace Application.Settings;
public class DatabaseSettings
{

    public DatabaseSettings(string databaseName = "DeviceDb", string databaseTable = "DeviceTable")
    {
        DatabaseName = string.IsNullOrEmpty(databaseName) ? throw new ArgumentNullException(databaseName, "DatabaseName cant be null") : databaseName;
        DatabaseTable = string.IsNullOrEmpty(databaseName) ? throw new ArgumentNullException(databaseTable, "DatabaseTable cant be null") : databaseTable;
    }

    public string ConnectionString => $"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\{DatabaseName}.sqlite";

    private string DatabaseName { get; init; }
    public string DatabaseTable { get; init; }
    public bool IsInitialized { get; set; } = false;
}
