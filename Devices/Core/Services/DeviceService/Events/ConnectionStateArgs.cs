namespace Core.Services.DeviceService.Events;

public class ConnectionStateArgs : EventArgs
{
    public ConnectionStateArgs(string message, bool isConnected)
    {
        Message = message;
        IsConnected = isConnected;
    }

    public string Message { get; }
    public bool IsConnected { get; }
}