﻿using Dapper;
using Microsoft.Data.Sqlite;
using Shared.Execeptions;
using Shared.Services.DatabaseService.Interfaces;
using Shared.Settings;

namespace Shared.Services.DatabaseService;

public class SqliteService : IDatabaseService
{
    private readonly DatabaseSettings _databaseSettings;

    public SqliteService(DatabaseSettings databaseSettings, DeviceSettings deviceSettings)
    {
        _databaseSettings = databaseSettings ?? throw new ArgumentNullException(nameof(databaseSettings));

        if (!_databaseSettings.IsInitialized)
        {
            SqliteBootstrapper.Setup(_databaseSettings, deviceSettings).ConfigureAwait(false);
        }
    }


    public async Task<DeviceSettings> GetDeviceSettings()
    {
        try
        {
            await using SqliteConnection conn = new(_databaseSettings.ConnectionString);
            return await conn.QueryFirstOrDefaultAsync<DeviceSettings>(
                $"SELECT * FROM {_databaseSettings.DatabaseTable}") ?? throw new DeviceSettingsException();
        }
        catch
        {
            throw new DeviceSettingsException();
        }
    }

    public async Task<bool> UpdateDeviceSettings(DeviceSettings settings)
    {
        try
        {
            await using SqliteConnection conn = new(_databaseSettings.ConnectionString);

            var sqlQuery = CreateUpdateEntitySqlQuery(settings, _databaseSettings.DatabaseTable);
            await conn.ExecuteAsync(sqlQuery, settings);

            return true;
        }
        catch
        {
            throw new DeviceSettingsException("could not update device settings");
        }
    }

    private static string CreateUpdateEntitySqlQuery(DeviceSettings deviceSettings, string databaseTable)
    {
        var update = $"UPDATE {databaseTable} SET ";

        List<string> properties = deviceSettings!.GetType().GetProperties().Select(x => x.Name).ToList();

        properties.ForEach(property =>
        {
            update += $"{property}=@{property}";
            update += properties.IndexOf(property) == properties.IndexOf(properties.Last()) ? " " : ",";
        });

        return update + $"WHERE {nameof(deviceSettings.DeviceId)} = @{nameof(deviceSettings.DeviceId)}";
    }
}