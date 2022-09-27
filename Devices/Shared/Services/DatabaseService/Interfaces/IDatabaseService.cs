using Shared.Settings;

namespace Shared.Services.DatabaseService.Interfaces;
public interface IDatabaseService
{
    Task<DeviceSettings> GetDeviceSettings(); 
    Task<bool> UpdateDeviceSettings(DeviceSettings settings);
}
