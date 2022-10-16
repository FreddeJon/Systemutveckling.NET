using Core.Settings;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Core.Services.DatabaseService;

public static class SqliteBootstrapper
{
    public static async Task Setup(DatabaseSettings databaseSettings, DeviceSettings deviceSettings)
    {
        await using SqliteConnection conn = new(databaseSettings.ConnectionString);
        try
        {
            string? table = (await conn.QueryAsync<string>(
                    $"SELECT name FROM sqlite_schema WHERE type = 'table' AND name = '{databaseSettings.DatabaseTable}'"))
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(table) && table == databaseSettings.DatabaseTable)
            {
                return;
            }

            await conn.ExecuteAsync(CreateTableSqlQueryForEntity(deviceSettings, databaseSettings.DatabaseTable));

            await conn.ExecuteAsync(CreateEntitySqlQuery(deviceSettings, databaseSettings.DatabaseTable), deviceSettings);

            databaseSettings.IsInitialized = true;
        }
        catch (Exception)
        {
            await conn.ExecuteAsync($"DROP TABLE[IF EXISTS] {databaseSettings.DatabaseTable};");
            throw new Exception("could not bootstrap db");
        }

    }


    private static string CreateTableSqlQueryForEntity(DeviceSettings deviceSettings, string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentNullException(tableName, "Table cant be null");
        }

        string sqlQuery = $"CREATE TABLE {tableName} (";

        List<string> properties = deviceSettings.GetType().GetProperties().Select(x => x.Name).ToList();

        foreach (string property in properties)
        {
            if (property == nameof(DeviceSettings.DeviceId))
            {
                sqlQuery += $" {property} VARCHAR(200) NOT NULL";
                sqlQuery +=
                    properties.IndexOf(property) != properties.IndexOf(properties.Last()) ? "," : ")";
            }
            else if (property == nameof(DeviceSettings.DeviceId))
            {
                sqlQuery += $" {property} INT NULL";
                sqlQuery +=
                    properties.IndexOf(property) != properties.IndexOf(properties.Last()) ? "," : ")";
            }
            else
            {
                sqlQuery += $" {property} VARCHAR(200) NULL";
                sqlQuery +=
                    properties.IndexOf(property) != properties.IndexOf(properties.Last()) ? "," : ")";
            }
        }


        return sqlQuery;
    }

    private static string CreateEntitySqlQuery(DeviceSettings settings, string tableName)
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
