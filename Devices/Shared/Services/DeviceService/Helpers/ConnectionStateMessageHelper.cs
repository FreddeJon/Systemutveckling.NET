using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services.DeviceService.Helpers;
public static class ConnectionStateMessageHelper
{
    public static string GetMessage(this ConnectionState connectionState)
    {
        return connectionState switch
        {
            ConnectionState.NotConnected => "Not connected",
            ConnectionState.Connecting => "Connecting",
            ConnectionState.StillConnecting => "Still connecting",
            ConnectionState.Initializing => "Initializing",
            ConnectionState.Connected => "Connected",
            _ => throw new ArgumentOutOfRangeException(nameof(connectionState), connectionState, null)
        };
    }
}
