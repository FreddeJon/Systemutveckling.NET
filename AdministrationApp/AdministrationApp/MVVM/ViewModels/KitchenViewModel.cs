using System.Threading.Tasks;
using AdministrationApp.Helpers;
using AdministrationApp.MVVM.Models;
using Core.Services;

namespace AdministrationApp.MVVM.ViewModels;

public class KitchenViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;
    private readonly DeviceListViewModel _deviceListViewModel;
    private readonly EditDeviceViewModel _editDeviceViewModel;
    private ViewModelBase _currentViewModel;

    public KitchenViewModel(IDeviceManager deviceManager, DeviceListViewModel deviceListViewModel, EditDeviceViewModel editDeviceViewModel)
    {
        _deviceManager = deviceManager;

        _deviceListViewModel = deviceListViewModel;
        _deviceListViewModel.CurrentRoom = Room.Kitchen;

        _editDeviceViewModel = editDeviceViewModel;
        _editDeviceViewModel.GoBackRequested += _editDeviceViewModel_GoBackRequested;

        _deviceListViewModel.EditDeviceRequested += _deviceListViewModel_EditDeviceRequested;


        CurrentViewModel = deviceListViewModel;
    }

    private void _editDeviceViewModel_GoBackRequested()
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


    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }
}