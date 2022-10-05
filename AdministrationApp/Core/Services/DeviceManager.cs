using Core.Models;
using Microsoft.Azure.Devices;

namespace Core.Services;

public class DeviceManager : IDeviceManager
{
    private readonly RegistryManager _registryManager;
    private string _currentRoom = "kitchen";
    private const string ConnectionString =
        "HostName=Systemutveckling-iot.azure-devices.net;SharedAccessKeyName=AdminApp;SharedAccessKey=coXYZ9vRJThoioshKnExrHZ//NJx3bqFar6wQLev31s=";

    public DeviceManager()
    {
        _registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
    }

    public Task ToggleDeviceStateAsync(string deviceId)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync()
    {
        var query = $"SELECT * FROM devices WHERE properties.reported.location = '{_currentRoom}";
        var result = _registryManager.CreateQuery("select * from devices WHERE properties.reported.location = 'kitchen'");

        if (result.HasMoreResults)
            foreach (var twin in await result.GetNextAsTwinAsync())
            {

                var device = new DeviceModel
                {
                    LastActivityTime =  twin.LastActivityTime,
                    ConnectionState = twin.ConnectionState,
                    DeviceId = twin.DeviceId
                };

                try
                {
                    device.DeviceName = twin.Properties.Reported["deviceName"];
                }
                catch
                {
                    device.DeviceName = device.DeviceId;
                }

                try
                {
                    device.DeviceType = twin.Properties.Reported["deviceType"];
                }
                catch
                {
                    // ignored
                }

                try
                {
                    device.DeviceActionState = twin.Properties.Reported["actionState"];
                }
                catch
                {
                    // ignored
                }

                switch (device.DeviceType.ToLower())
                {
                    case "fan":
                        device.IconActiveState = "\uf863";
                        device.IconInActiveState = "\uf863";
                        device.TextActiveState = "ON";
                        device.TextInActiveState = "OFF";
                        break;

                    case "light":
                        device.IconActiveState = "\uf672";
                        device.IconInActiveState = "\uf0eb";
                        device.TextActiveState = "ON";
                        device.TextInActiveState = "OFF";
                        break;

                    default:
                        device.IconActiveState = "\uf2db";
                        device.IconInActiveState = "\uf2db";
                        device.TextActiveState = "ENABLE";
                        device.TextInActiveState = "DISABLE";
                        break;
                }

                yield return device;
            }
    }
}