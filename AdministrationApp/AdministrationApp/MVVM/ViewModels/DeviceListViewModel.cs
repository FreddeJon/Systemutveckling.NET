using AdministrationApp.Helpers;
using AdministrationApp.MVVM.Models;
using Core.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Microsoft.Azure.Devices;

namespace AdministrationApp.MVVM.ViewModels;

public sealed class DeviceListViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;

    private ObservableCollection<DeviceItem> _devices;
    private CancellationTokenSource? _cts;
    private readonly IMapper _mapper;
    private List<DeviceItem> _tempList = new();


    public DeviceListViewModel(IDeviceManager deviceManager, IMapper mapper)
    {
        _deviceManager = deviceManager;
        _devices = new ObservableCollection<DeviceItem>();

        _mapper = mapper;


        ToggleDeviceActionStateCommand = new RelayCommand<DeviceItem>(ToggleDevice);
        EditDeviceCommand = new RelayCommand<DeviceItem>(OnEditDeviceRequested);
    }

    public Room CurrentRoom { get; set; } = Room.Unset;

    public RelayCommand<DeviceItem> EditDeviceCommand { get; set; }

    public RelayCommand<DeviceItem> ToggleDeviceActionStateCommand { get; set; }

    public event Action<DeviceItem> EditDeviceRequested = delegate { };


    public ObservableCollection<DeviceItem> Devices
    {
        get => _devices;
        set => SetProperty(ref _devices, value);
    }

    public override async Task LoadAsync()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

        try
        {
            _cts = new CancellationTokenSource();

            do
            {
                var devices = await _deviceManager.GetDevicesAsync(CurrentRoom.GetRoom());


                _tempList.Clear();

                foreach (var item in _devices)
                {
                    var device = devices.FirstOrDefault(x => x.DeviceId == item.DeviceId);
                    if (device == null)
                        _tempList.Add(item);
                }

                foreach (var item in _tempList)
                {
                    _devices.Remove(item);
                }


                foreach (var device in devices)
                {
                    var tempDevice = Devices.FirstOrDefault(x => x.DeviceId == device.DeviceId);
                    if (tempDevice is null)
                    {
                        Devices.Add(_mapper.Map<DeviceItem>(device));
                    }
                    else
                    {
                        var mappedDevice = _mapper.Map<DeviceItem>(device);
                        tempDevice = mappedDevice;
                    }

                }

                await Task.Delay(1000);
            } while (await timer.WaitForNextTickAsync(_cts.Token));
        }
        catch
        {
            // ignored
        }
    }

    private async void ToggleDevice(DeviceItem? device)
    {
        if (device is null)
            return;

        await _deviceManager.ToggleDeviceStateAsync(device.DeviceId, device.ActionState);
    }

    private void OnEditDeviceRequested(DeviceItem? device)
    {
        if (device is null)
            return;
        if (_cts is null)
        {
            Console.WriteLine();
        }
        _cts?.Cancel();

        EditDeviceRequested(device);
    }
}