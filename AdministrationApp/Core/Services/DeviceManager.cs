using Core.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace Core.Services;

public class DeviceManager : IDeviceManager
{
    private const string CurrentRoom = "kitchen";

    private const string ConnectionString =
        "HostName=Systemutveckling-iot.azure-devices.net;SharedAccessKeyName=AdminApp;SharedAccessKey=coXYZ9vRJThoioshKnExrHZ//NJx3bqFar6wQLev31s=";

    private readonly RegistryManager _registryManager;
    private readonly ServiceClient _serviceManager;

    public DeviceManager()
    {
        _serviceManager = ServiceClient.CreateFromConnectionString(ConnectionString);
        _registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
    }

    public async Task<bool> ToggleDeviceStateAsync(string deviceId, bool updatedState)
    {
        var twin = await _registryManager.GetDeviceAsync(deviceId);

        if (twin is null || twin.ConnectionState != DeviceConnectionState.Connected) return false;
        var stateProp = updatedState ? "true" : "false";
        var toggleActionState = new CloudToDeviceMethod("ChangeActionState");
        toggleActionState.SetPayloadJson(stateProp);

        try
        {
            await _serviceManager.InvokeDeviceMethodAsync(deviceId, toggleActionState);
            return true;
        }
        catch
        {
            // ignored
        }

        return false;
    }

    public async Task<IEnumerable<DeviceModel>> GetDevicesAsync(string room)
    {
        var query = room == "unset"
            ? "select * from devices"
            : $"select * from devices WHERE properties.reported.location = '{room}'";

        var result =
            _registryManager.CreateQuery(query);
        var devices = new List<DeviceModel>();
        if (result.HasMoreResults)
            foreach (var twin in await result.GetNextAsTwinAsync())
            {
                var device = new DeviceModel
                {
                    LastActivityTime = twin.LastActivityTime,
                    ConnectionState = twin.ConnectionState == DeviceConnectionState.Connected,
                    DeviceId = twin.DeviceId
                };

                device = GetReportedProperties(device, twin);

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
                    case "ac":
                        device.IconActiveState = "\uf8f4";
                        device.IconInActiveState = "\uf8f4";
                        device.TextActiveState = "ON";
                        device.TextInActiveState = "OFF";
                        break;
                    case "sensor":
                        device.IconActiveState = "\uf769";
                        device.IconInActiveState = "\uf769";
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

                devices.Add(device);
            }

        return devices;
    }

    public async Task DeleteDeviceAsync(DeviceModel device)
    {
        try
        {
            await _registryManager.RemoveDeviceAsync(device.DeviceId);
        }
        catch
        {
            // ignored
        }
    }

    public async Task<bool> UpdateDeviceAsync(DeviceModel device)
    {
        var twin = await _registryManager.GetTwinAsync(device.DeviceId);

        if (twin is null || twin.ConnectionState != DeviceConnectionState.Connected) return false;

        var newDevice = GetReportedProperties(new DeviceModel(), twin);


        var json = JsonConvert.SerializeObject(new
        {
            device.DeviceName,
            DeviceType = device.DeviceType.ToLower(),
            device.Location,
            device.Owner,
            device.Interval
        });


        if (CheckIfEqual(device, newDevice)) return false;


        var updateDevice = new CloudToDeviceMethod("UpdateDeviceProperties");
        updateDevice.SetPayloadJson(json);
        await _serviceManager.InvokeDeviceMethodAsync(device.DeviceId, updateDevice);

        return true;
    }


    private bool CheckIfEqual(DeviceModel first, DeviceModel second)
    {
        var isEqual = first.DeviceName == second.DeviceName;


        if (first.DeviceType != second.DeviceType) isEqual = false;

        if (first.Location != second.Location) isEqual = false;

        if (first.Owner != second.Owner) isEqual = false;

        if (first.Interval != second.Interval) isEqual = false;

        return isEqual;
    }

    private DeviceModel GetReportedProperties(DeviceModel device, Twin twin)
    {
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
            device.DeviceType = char.ToUpper(device.DeviceType[0]) + device.DeviceType[1..];
        }
        catch
        {
            // ignored
        }

        try
        {
            device.Location = twin.Properties.Reported["location"];

            device.Location = char.ToUpper(device.Location[0]) + device.Location[1..];
        }
        catch
        {
            // Ignored
        }

        try
        {
            device.Owner = twin.Properties.Reported["owner"];
        }
        catch
        {
            // Ignored
        }

        try
        {
            device.Interval = twin.Properties.Reported["interval"];
        }
        catch
        {
            // Ignored
        }

        try
        {
            device.ActionState = twin.Properties.Reported["actionState"];
        }
        catch
        {
            // ignored
        }

        return device;
    }
}