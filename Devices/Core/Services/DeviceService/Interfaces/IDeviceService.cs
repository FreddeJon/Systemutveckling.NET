using Core.Services.DeviceService.Events;

namespace Core.Services.DeviceService.Interfaces;
public interface IDeviceService
{
    public Task ConnectDeviceAsync();
    public Task SendMessageAsync(object data);
    public bool ChangeSendingAllowed(bool isAllowed);

    public bool IsAllowedToSendMessages { get; }

    public event EventHandler<string> DeviceServiceErrorEvent;
    public event EventHandler<ConnectionStateArgs> DeviceConnectionStateChangedEvent;
    public event EventHandler<SendingMessagesArgs> DeviceActionStateChangedEvent;
}
