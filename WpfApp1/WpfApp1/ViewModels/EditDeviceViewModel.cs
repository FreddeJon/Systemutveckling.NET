using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class EditDeviceViewModel : ObservableObject
    {
        public string DeviceName { get; set; }

        public EditDeviceViewModel()
        {
            GoBackCommand = new RelayCommand(OnGoBack);
        }

        public RelayCommand GoBackCommand { get; set; }

        public event Action GoBackRequested = delegate { };

        private void OnGoBack(object? obj)
        {
            GoBackRequested();
        }
    }
}
