namespace Application.Services.DeviceService.Events;

public delegate void ConnectionStateChangedEventHandler(object sender, ConnectionStateArgs e);
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