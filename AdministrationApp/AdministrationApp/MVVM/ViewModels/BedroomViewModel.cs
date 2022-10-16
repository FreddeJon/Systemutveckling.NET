using AdministrationApp.Helpers;
using AdministrationApp.MVVM.Models;
using System.Threading.Tasks;

// ReSharper disable MemberCanBePrivate.Global

namespace AdministrationApp.MVVM.ViewModels;

public class BedroomViewModel : ViewModelBase
{
    private readonly DeviceListViewModel _deviceListViewModel;
    private readonly EditDeviceViewModel _editDeviceViewModel;
    private ViewModelBase _currentViewModel = null!;

    public BedroomViewModel(DeviceListViewModel deviceListViewModel, EditDeviceViewModel editDeviceViewModel, WeatherViewModel weatherViewModel)
    {
        _deviceListViewModel = deviceListViewModel;
        _deviceListViewModel.CurrentRoom = Room.Bedroom;

        _editDeviceViewModel = editDeviceViewModel;
        _editDeviceViewModel.GoBackRequested += _editDeviceViewModel_GoBackRequested;
        _deviceListViewModel.EditDeviceRequested += _deviceListViewModel_EditDeviceRequested;


        CurrentViewModel = deviceListViewModel;
        WeatherViewModel = weatherViewModel;
    }

    public WeatherViewModel WeatherViewModel { get; set; }


    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    private void _editDeviceViewModel_GoBackRequested(DeviceItem? device)
    {
        SetCurrentViewModel(_deviceListViewModel);
    }

    private void _deviceListViewModel_EditDeviceRequested(DeviceItem? device)
    {
        if (device is null)
            return;

        _editDeviceViewModel.Device = device;
        SetCurrentViewModel(_editDeviceViewModel);
    }

    public override async Task LoadAsync()
    {
        await CurrentViewModel.LoadAsync();
    }

    private async void SetCurrentViewModel(ViewModelBase view)
    {
        CurrentViewModel = view;
        await CurrentViewModel.LoadAsync();
    }


    public override void Dispose()
    {
        _editDeviceViewModel.GoBackRequested -= _editDeviceViewModel_GoBackRequested;
        _deviceListViewModel.EditDeviceRequested -= _deviceListViewModel_EditDeviceRequested;
        base.Dispose();
    }
}