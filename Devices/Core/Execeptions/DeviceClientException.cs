namespace Core.Execeptions;
public class DeviceClientException : Exception
{
    public DeviceClientException() : base("Error with deviceclient")
    {

    }
    public DeviceClientException(string message) : base(message)
    {

    }
}
