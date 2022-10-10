namespace Core.Services.DeviceService.Events;

public class SendingMessagesArgs : EventArgs
{
    public bool IsAllowedToSend { get; }

    public SendingMessagesArgs(bool isAllowedToSend)
    {
        IsAllowedToSend = isAllowedToSend;
    }
}