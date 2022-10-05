using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdministrationApp.MVVM.Models;
using Core.Models;
using Core.Services;

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

    public IReadOnlyList<string> Types { get; set; } = new List<string> {"Fan", "Light", "Sensor", "Temperature","Unkown"};
    public IReadOnlyList<string> Locations { get; set; } = new List<string> {"Kitchen", "Hallway", "Bedroom", "Livingroom","Unkown"};



    public AsyncRelayCommand<DeviceItem> EditDeviceCommand { get; set; }
    public RelayCommand GoBackCommand { get; set; }


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