using Core.Models;

namespace Core.Services;

public interface IDeviceManager
{
    public Task ToggleDeviceStateAsync(string deviceId);
    public IAsyncEnumerable<DeviceModel> GetDevicesAsync();

}