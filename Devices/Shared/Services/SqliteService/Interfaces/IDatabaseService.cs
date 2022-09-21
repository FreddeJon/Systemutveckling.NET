using Shared.Settings;

namespace Shared.Services.SqliteService.Interfaces;
public interface IDatabaseService
{
    Task<DeviceSettingsBase> GetDeviceSettings(); 
    Task UpdateDeviceSettings(DeviceSettingsBase settings);
}
