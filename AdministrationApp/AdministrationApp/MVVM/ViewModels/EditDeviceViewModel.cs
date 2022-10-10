using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdministrationApp.MVVM.Models;
using Core.Models;
using Core.Services;
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
        GoBackCommand = new RelayCommand(OnGoBackRequested);
        EditDeviceCommand = new AsyncRelayCommand<DeviceItem>(EditDeviceRequested);
    }

    public bool DeviceIsOnline => Device?.ConnectionState == "Connected";

    public DeviceItem? Device
    {
        get => _device;
        set
        {
            if (SetProperty(ref _device, value))
            {
                OnPropertyChanged(nameof(DeviceIsOnline));
            }
        }
    }

    public IReadOnlyList<string> Types { get; init; } = new List<string> { "Fan", "Light", "Ac", "Sensor", "Unkown" };
    public IReadOnlyList<string> Locations { get; init; } = new List<string> { "Kitchen", "Bedroom", "Livingroom", "Unkown" };



    public AsyncRelayCommand<DeviceItem> EditDeviceCommand { get; }
    public RelayCommand GoBackCommand { get; }


    public event Action GoBackRequested = delegate { };

    private void OnGoBackRequested()
    {
        GoBackRequested();
    }
    private async Task EditDeviceRequested(DeviceItem? obj)
    {
        if (obj is null) return;
        await _deviceManager.UpdateDeviceAsync(_mapper.Map<DeviceModel>(obj));
        GoBackRequested();
    }
}