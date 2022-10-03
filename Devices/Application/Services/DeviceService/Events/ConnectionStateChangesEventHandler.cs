namespace Application.Services.DeviceService.Events;

public delegate void ConnectionStateChangesEventHandler(object sender, ConnectionStateArgs e);
public class ConnectionStateArgs : EventArgs
{
    public ConnectionStateArgs(string message, bool isConnected, string? errorMessage = null)
    {
        Message = message;
        IsConnected = isConnected;
        ErrorMessage = errorMessage;
    }

    public string Message { get; }
    public bool IsConnected { get; }
    public string? ErrorMessage { get; }
}