using Dapper;
using Microsoft.Data.Sqlite;
using Shared.Services.SqliteService.Interfaces;
using Shared.Settings;

namespace Shared.Services.SqliteService;

public class SqliteService : IDatabaseService
{
    private readonly DatabaseSettings _databaseSettings;
    private readonly DeviceSettingsBase _deviceSettings;

    public SqliteService(DatabaseSettings databaseSettings, DeviceSettingsBase deviceSettings)
    {
        _databaseSettings = databaseSettings ?? throw new ArgumentNullException(nameof(databaseSettings));
        _deviceSettings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));

        if (!_databaseSettings.IsInitialized)
        {
            SqliteBootstrapper.Setup(_databaseSettings, deviceSettings).ConfigureAwait(false);
        }
    }


    public async Task<DeviceSettingsBase> GetDeviceSettings()
    {
        try
        {
            await using SqliteConnection conn = new(_databaseSettings.ConnectionString);
            return await conn.QueryFirstOrDefaultAsync<DeviceSettingsBase>(
                $"SELECT * FROM {_databaseSettings.DatabaseTable}");
        }
        catch
        {
            // ignored
        }

        return null!;
    }

    public async Task UpdateDeviceSettings(DeviceSettingsBase settings)
    {
        try
        {
            await using SqliteConnection conn = new(_databaseSettings.ConnectionString);

            var sqlQuery = CreateUpdateEntitySqlQuery(_deviceSettings, _databaseSettings.DatabaseTable);
            await conn.ExecuteAsync(sqlQuery, _deviceSettings);
        }
        catch
        {
            // ignored
        }
    }

    private static string CreateUpdateEntitySqlQuery(DeviceSettingsBase deviceSettings, string databaseTable)
    {
        if (string.IsNullOrEmpty(databaseTable))
        {
            throw new ArgumentNullException(databaseTable, "DatabaseTable is required");
        }

        var update = $"UPDATE {databaseTable} SET ";

        List<string> properties = deviceSettings.GetType().GetProperties().Select(x => x.Name).ToList();

        properties.ForEach(property =>
        {
            update += $"{property}=@{property}";
            update += properties.IndexOf(property) == properties.IndexOf(properties.Last()) ? " " : ",";
        });

        return update + $"WHERE {nameof(deviceSettings.DeviceId)} = @{nameof(deviceSettings.DeviceId)}";
    }
}