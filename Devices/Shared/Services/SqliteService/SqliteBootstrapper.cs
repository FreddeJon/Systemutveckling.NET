using Dapper;
using Microsoft.Data.Sqlite;
using Shared.Services.SqliteService.Interfaces;
using Shared.Settings;

namespace Shared.Services.SqliteService;

public static class SqliteBootstrapper
{
    public static async Task Setup(DatabaseSettings databaseSettings, DeviceSettingsBase deviceSettings)
    {
        try
        {
            await using SqliteConnection conn = new(databaseSettings.ConnectionString);
            string? table = (await conn.QueryAsync<string>(
                    $"SELECT name FROM sqlite_master WHERE type = 'tableName' AND name = '{databaseSettings.DatabaseTable}'"))
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(table) && table == databaseSettings.DatabaseTable)
            {
                return;
            }

            await conn.ExecuteAsync(CreateTableSqlQueryForEntity(deviceSettings, databaseSettings.DatabaseTable));

            await conn.ExecuteAsync(CreateEntitySqlQuery(deviceSettings, databaseSettings.DatabaseTable));

            databaseSettings.IsInitialized = true;
        }
        catch (Exception e)
        {
            throw;
        }

    }


    private static string CreateTableSqlQueryForEntity(DeviceSettingsBase deviceSettings, string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentNullException(tableName, "Table cant be null");
        }

        string sqlQuery = $"CREATE TABLE {tableName} (";

        List<string> properties = deviceSettings.GetType().GetProperties().Select(x => x.Name).ToList();

        foreach (string property in properties)
        {
            if (property == nameof(DeviceSettingsBase.DeviceId))
            {
                sqlQuery += $" {property} VARCHAR(200) NOT NULL";
                sqlQuery +=
                    properties.IndexOf(property) != properties.IndexOf(properties.Last()) ? "," : ");";
            }
            else
            {
                sqlQuery += $" {property} VARCHAR(200) NULL";
                sqlQuery +=
                    properties.IndexOf(property) != properties.IndexOf(properties.Last()) ? "," : ");";
            }
        }


        return sqlQuery;
    }

    private static string CreateEntitySqlQuery(DeviceSettingsBase settings, string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentNullException(tableName, "Table cant be null");
        }

        List<string> properties = settings.GetType().GetProperties().Select(propertyInfo => propertyInfo.Name)
            .ToList();

        string insertInto = $"INSERT INTO {tableName} (";
        string values = " VALUES(";

        properties.ForEach(property =>
        {
            if (properties.IndexOf(property) != properties.IndexOf(properties.Last()))
            {
                insertInto += $"{property}, ";
                values += $"@{property}, ";
            }
            else
            {
                insertInto += $"{property})";
                values += $"@{property})";
            }
        });


        string sqlQuery = insertInto + values;

        return sqlQuery;
    }
}

public class BootstrapperArgs : EventArgs
{
    public DeviceSettingsBase DeviceSettings { get; }
    public DatabaseSettings Settings { get; }

    public BootstrapperArgs(DeviceSettingsBase deviceSettings, DatabaseSettings settings)
    {
        DeviceSettings = deviceSettings;
        Settings = settings;
    }
}