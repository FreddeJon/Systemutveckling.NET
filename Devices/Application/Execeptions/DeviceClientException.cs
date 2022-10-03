using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Execeptions;
public class DeviceClientException : Exception
{
    public DeviceClientException() : base("error with deviceclient")
    {
        
    }
    public DeviceClientException(string message) : base(message)
    {
        
    }
}
