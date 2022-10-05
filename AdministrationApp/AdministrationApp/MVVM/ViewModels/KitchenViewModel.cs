using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdministrationApp.MVVM.Models;
using Core.Services;

namespace AdministrationApp.MVVM.ViewModels;

public class KitchenViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;

    private ObservableCollection<DeviceItem> _device;

    public KitchenViewModel(IDeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
        _device = new ObservableCollection<DeviceItem>();

        ToggleDeviceActionStateCommand = new RelayCommand<DeviceItem>(ToggleDevice);
    }


    public RelayCommand<DeviceItem> ToggleDeviceActionStateCommand { get; set; }

    public ObservableCollection<DeviceItem> Devices
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }

    public override async Task LoadAsync()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

        do
        {
            await foreach (var device in _deviceManager.GetDevicesAsync())
            {
                var tempDevice = Devices.FirstOrDefault(x => x.DeviceId == device.DeviceId);
                if (tempDevice is null)
                {
                    Devices.Add(new DeviceItem
                    {
                        DeviceId = device.DeviceId,
                        DeviceName = device.DeviceName,
                        DeviceType = device.DeviceType,
                        ActionState = device.DeviceActionState,
                        IconActiveState = device.IconActiveState,
                        IconInActiveState = device.IconInActiveState,
                        TextActiveState = device.TextActiveState,
                        TextInActiveState = device.TextInActiveState
                    });
                }
                else
                {
                    tempDevice.DeviceId = device.DeviceId;
                    tempDevice.DeviceName = device.DeviceName;
                    tempDevice.DeviceType = device.DeviceType;
                    tempDevice.ActionState = device.DeviceActionState;
                    tempDevice.IconActiveState = device.IconActiveState;
                    tempDevice.IconInActiveState = device.IconInActiveState;
                    tempDevice.TextActiveState = device.TextActiveState;
                    tempDevice.TextInActiveState = device.TextInActiveState;
                }
            }

            await Task.Delay(1000);
        } while (await timer.WaitForNextTickAsync());
    }


    //private async void ToggleActionStateEventHandler(object? sender, ToggleActionStateArgs e)
    //{
    //    var stateProp = e.State ? "true" : "false";

    //    var toggleActionState = new CloudToDeviceMethod("ChangeActionState");
    //    toggleActionState.SetPayloadJson(stateProp);
    //    //await _serviceManager.InvokeDeviceMethodAsync(e.DeviceId, toggleActionState);
    //}


    private void ToggleDevice(DeviceItem? device)
    {
        //throw new NotImplementedException();
    }
}