using System;
using System.Threading;
using System.Threading.Tasks;
using AdministrationApp.Helpers;
using AdministrationApp.MVVM.Models;
using Core.Services.WeatherService;

// ReSharper disable MemberCanBePrivate.Global

namespace AdministrationApp.MVVM.ViewModels;

public class KitchenViewModel : ViewModelBase
{
    private readonly DeviceListViewModel _deviceListViewModel;
    private readonly EditDeviceViewModel _editDeviceViewModel;
    private ViewModelBase _currentViewModel = null!;

    public KitchenViewModel(DeviceListViewModel? deviceListViewModel, EditDeviceViewModel? editDeviceViewModel, WeatherViewModel weatherViewModel)
    {
        _deviceListViewModel = deviceListViewModel ?? throw new ArgumentNullException(nameof(deviceListViewModel));
        _editDeviceViewModel = editDeviceViewModel ?? throw new ArgumentNullException(nameof(editDeviceViewModel));


        deviceListViewModel.CurrentRoom = Room.Kitchen;
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

    private async void SetCurrentViewModel(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
        await CurrentViewModel.LoadAsync();
    }
}