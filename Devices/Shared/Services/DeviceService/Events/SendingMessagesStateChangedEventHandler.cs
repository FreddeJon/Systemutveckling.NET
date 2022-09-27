namespace Shared.Services.DeviceService.Events;

public delegate void SendingMessagesStateChangedEventHandler(object sender, SendingMessagesArgs e);

public class SendingMessagesArgs : EventArgs
{
    public bool IsAllowedToSend { get; }

    public SendingMessagesArgs(bool isAllowedToSend)
    {
        IsAllowedToSend = isAllowedToSend;
    }
}