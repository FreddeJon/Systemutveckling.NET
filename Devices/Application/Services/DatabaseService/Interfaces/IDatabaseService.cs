using Application.Settings;

namespace Application.Services.DatabaseService.Interfaces;
public interface IDatabaseService
{
    Task<DeviceSettings> GetDeviceSettings(); 
    Task<bool> UpdateDeviceSettings(DeviceSettings settings);
}
