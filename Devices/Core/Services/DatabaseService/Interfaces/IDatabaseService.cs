using Core.Settings;

namespace Core.Services.DatabaseService.Interfaces;
public interface IDatabaseService
{
    Task<DeviceSettings> GetDeviceSettingsAsync(); 
    Task<bool> UpdateDeviceSettingsAsync(DeviceSettings settings);

    Task<bool> ResetConnectionStringAsync(DeviceSettings? settings);
}
