using Core.Models;

namespace Core.Services;

public interface IDeviceManager
{
    public Task<bool> ToggleDeviceStateAsync(string deviceId, bool updatedState);
    public Task<IEnumerable<DeviceModel>> GetDevicesAsync(string room);
    public Task DeleteDeviceAsync(DeviceModel device);
    Task<bool> UpdateDeviceAsync(DeviceModel device);
}