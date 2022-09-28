using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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


    private readonly ObservableCollection<DeviceItem> _deviceItems;


    public KitchenViewModel()
    {
        _deviceItems = new ObservableCollection<DeviceItem>();
        PopulateDeviceItemsAsync().ConfigureAwait(false);

        ToggleRunningState += ToggleRunningStateHandler;


    }

    private async void ToggleRunningStateHandler(object? sender, ToggleSendingStateArgs e)
    {
        var send = e.ShouldSend ? "true" : "false";

        var cloudToDeviceMethod = new CloudToDeviceMethod("ChangeSendingState");
        cloudToDeviceMethod.SetPayloadJson(send);
       await _serviceManager.InvokeDeviceMethodAsync(e.DeviceId, cloudToDeviceMethod);
    }

    public IEnumerable<DeviceItem> DeviceItems => _deviceItems;



    public event EventHandler<ToggleSendingStateArgs> ToggleRunningState; 

    private async Task PopulateDeviceItemsAsync()
    {
        var result = _registryManager.CreateQuery("select * from devices");

        if (result.HasMoreResults)
            foreach (var twin in await result.GetNextAsTwinAsync())
            {
                var device = _deviceItems.FirstOrDefault(x => x.DeviceId == twin.DeviceId);

                if (device == null)
                {
                    device = new DeviceItem
                    {
                        HandleToggleState = ToggleRunningState,
                        DeviceId = twin.DeviceId
                    };

                    try
                    {
                        device.IsChecked = twin.Properties.Reported["running"];
                    }
                    catch
                    {
                        device.IsChecked = false;
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
                        // ignored
                    }

                    switch (device.DeviceType.ToLower())
                    {
                        case "fan":
                            device.IconActive = "\uf863";
                            device.IconInActive = "\uf863";
                            device.StateActive = "ON";
                            device.StateInActive = "OFF";
                            break;

                        case "light":
                            device.IconActive = "\uf672";
                            device.IconInActive = "\uf0eb";
                            device.StateActive = "ON";
                            device.StateInActive = "OFF";
                            break;

                        default:
                            device.IconActive = "\uf2db";
                            device.IconInActive = "\uf2db";
                            device.StateActive = "ENABLE";
                            device.StateInActive = "DISABLE";
                            break;
                    }

                    _deviceItems.Add(device);
                }
            }
        else
            _deviceItems.Clear();
    }
}


public class ToggleSendingStateArgs : EventArgs
{
    public string DeviceId { get; }
    public bool ShouldSend { get; }

    public ToggleSendingStateArgs(string deviceId, bool shouldSend)
    {
        DeviceId = deviceId;
        ShouldSend = shouldSend;
    }
}