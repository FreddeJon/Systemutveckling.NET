using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class KitchenViewModel : ObservableObject
    {
        private readonly EditDeviceViewModel _editDeviceViewModel;
        private readonly DeviceListViewModel _deviceListViewModel;
        private ObservableObject _currentView;

        public KitchenViewModel()
        {
            _deviceListViewModel = new DeviceListViewModel();
            _editDeviceViewModel = new EditDeviceViewModel();

            _deviceListViewModel.EditDeviceItemRequested += _deviceListViewModel_EditDeviceItemRequested;
            _editDeviceViewModel.GoBackRequested += EditDeviceViewModelOnGoBackRequested;

            CurrentView = _deviceListViewModel;
        }

        private void EditDeviceViewModelOnGoBackRequested()
        {
            CurrentView = _deviceListViewModel;
        }

        private void _deviceListViewModel_EditDeviceItemRequested(DeviceItem obj)
        {

            _editDeviceViewModel.DeviceName = obj.DeviceName;
            CurrentView = _editDeviceViewModel;
        }

        public ObservableObject CurrentView
        {
            get => _currentView;
            set => SetField(ref _currentView, value);
        }
    }
}
