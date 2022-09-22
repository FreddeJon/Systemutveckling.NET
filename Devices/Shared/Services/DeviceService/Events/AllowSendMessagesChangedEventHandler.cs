namespace Shared.Services.DeviceService.Events;

public delegate void AllowSendMessagesChangedEventHandler(object sender, AllowSendMessagesArgs e);

public class AllowSendMessagesArgs : EventArgs
{
    public bool IsAllowed { get; }

    public AllowSendMessagesArgs(bool isAllowed)
    {
        IsAllowed = isAllowed;
    }
}