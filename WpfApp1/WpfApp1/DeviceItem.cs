using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DeviceItem
    {
        public DeviceItem(string deviceName)
        {
            DeviceName = deviceName;
        }

        public string DeviceName { get; set; }
    }
}
