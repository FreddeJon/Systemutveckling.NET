using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services.DeviceService.Helpers;
public enum ConnectionState
{
    NotConnected,
    Connecting,
    StillConnecting,
    Initializing,
    Connected
}
