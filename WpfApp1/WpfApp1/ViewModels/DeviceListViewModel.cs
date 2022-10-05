using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class DeviceListViewModel : ObservableObject
    {
        public List<DeviceItem> Devices { get; set; } = new List<DeviceItem>() { new DeviceItem("1"),  new DeviceItem("2"), new DeviceItem("3")};

        public DeviceListViewModel()
        {
            EditDeviceItemCommand = new RelayCommand(OnEditDeviceItem);
        }

        public RelayCommand EditDeviceItemCommand { get; set; }


        public event Action<DeviceItem> EditDeviceItemRequested = delegate {  };

        private void OnEditDeviceItem(object? obj)
        {
            var deviceItem = obj as DeviceItem;
            EditDeviceItemRequested(deviceItem);
        }
    }
}
