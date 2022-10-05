using Core.Models;

namespace Core.Services;

public interface IDeviceManager
{
    public Task<bool> ToggleDeviceStateAsync(string deviceId, bool updatedState);
    public Task<IEnumerable<DeviceModel>> GetDevicesAsync(string room);

    Task<bool> UpdateDeviceAsync(DeviceModel device);
}