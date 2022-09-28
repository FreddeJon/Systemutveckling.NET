using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdministrationApp.Events;
using AdministrationApp.MVVM.Core;
using AdministrationApp.MVVM.Models;
using Microsoft.Azure.Devices;

namespace AdministrationApp.MVVM.ViewModels;

public class KitchenViewModel : ObservableObject
{
    private readonly RegistryManager _registryManager = RegistryManager.CreateFromConnectionString(
        "HostName=Systemutveckling-iot.azure-devices.net;SharedAccessKeyName=AdminApp;SharedAccessKey=coXYZ9vRJThoioshKnExrHZ//NJx3bqFar6wQLev31s=");

    private readonly ServiceClient _serviceManager = ServiceClient.CreateFromConnectionString(
        "HostName=Systemutveckling-iot.azure-devices.net;SharedAccessKeyName=AdminApp;SharedAccessKey=coXYZ9vRJThoioshKnExrHZ//NJx3bqFar6wQLev31s=");


    private List<DeviceItem> _device;


    public KitchenViewModel()
    {
        _device = new List<DeviceItem>();
        ToggleActionStateEvent += ToggleActionStateEventHandler;
    }

    public List<DeviceItem> Devices
    {
        get => _device;
        set
        {
            if (Equals(value, _device)) return;
            _device = value;
            OnPropertyChanged();
        }
    }


    public override async Task LoadAsync()
    {


        var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

        do
        {
            Devices.Clear();

            Devices = await PopulateDeviceItemsAsync();

            await Task.Delay(1000);

        } while (await timer.WaitForNextTickAsync());
    }

    private event EventHandler<ToggleActionStateArgs> ToggleActionStateEvent;


    private async void ToggleActionStateEventHandler(object? sender, ToggleActionStateArgs e)
    {
        var stateProp = e.State ? "true" : "false";

        var toggleActionState = new CloudToDeviceMethod("ChangeSendingState");
        toggleActionState.SetPayloadJson(stateProp);
        await _serviceManager.InvokeDeviceMethodAsync(e.DeviceId, toggleActionState);
    }


    private async Task<List<DeviceItem>> PopulateDeviceItemsAsync()
    {
        var result = _registryManager.CreateQuery("select * from devices WHERE properties.reported.location = 'kitchen'");

        var devices = new List<DeviceItem>();

        if (result.HasMoreResults)
            foreach (var twin in await result.GetNextAsTwinAsync())
            {
                var device = _device.FirstOrDefault(x => x.DeviceId == twin.DeviceId);

                if (device != null) continue;

                device = new DeviceItem
                {
                    ToggleActionStateEvent = ToggleActionStateEvent,
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
                    device.DeviceName = twin.Properties.Reported["deviceName"];
                }
                catch
                {
                    device.DeviceName = device.DeviceId;
                }

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
                    device.DeviceType = "Unknown";
                }

                try
                {
                    device.ActionState = twin.Properties.Reported["actionState"];
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

                devices.Add(device);
            }

        return devices;
    }
}