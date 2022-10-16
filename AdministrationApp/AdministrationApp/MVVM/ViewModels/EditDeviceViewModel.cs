using AdministrationApp.MVVM.Models;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services.DeviceService;

// ReSharper disable MemberCanBePrivate.Global

namespace AdministrationApp.MVVM.ViewModels;

public class EditDeviceViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;
    private readonly IMapper _mapper;
    private DeviceItem? _device;

    public EditDeviceViewModel(IDeviceManager deviceManager, IMapper mapper)
    {
        _deviceManager = deviceManager;
        _mapper = mapper;
        GoBackCommand = new RelayCommand<DeviceItem>(OnGoBackRequested);
        EditDeviceCommand = new AsyncRelayCommand<DeviceItem>(OnEditDeviceRequested);
        DeleteDeviceCommand = new AsyncRelayCommand<DeviceItem>(OnDeleteDeviceRequested);
    }


    public AsyncRelayCommand<DeviceItem> DeleteDeviceCommand { get; }


    public DeviceItem? Device
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }

    public IReadOnlyList<string> Types { get; init; } = new List<string> { "Fan", "Light", "Ac", "Sensor", "Unkown" };
    public IReadOnlyList<string> Locations { get; init; } = new List<string> { "Kitchen", "Bedroom" };


    public AsyncRelayCommand<DeviceItem> EditDeviceCommand { get; }
    public RelayCommand<DeviceItem> GoBackCommand { get; }


    public event Action<DeviceItem?> GoBackRequested = delegate { };

    private void OnGoBackRequested(DeviceItem? deviceItem)
    {
        GoBackRequested(null);
    }

    private async Task OnDeleteDeviceRequested(DeviceItem? device)
    {
        if (device is null) return;
        await _deviceManager.DeleteDeviceAsync(_mapper.Map<DeviceModel>(device));
        GoBackRequested(device);
    }

    private async Task OnEditDeviceRequested(DeviceItem? device)
    {
        if (device is null) return;
        await _deviceManager.UpdateDeviceAsync(_mapper.Map<DeviceModel>(device));
        GoBackRequested(device);
    }
}