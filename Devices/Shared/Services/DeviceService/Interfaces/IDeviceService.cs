using Shared.Services.DeviceService.Events;

namespace Shared.Services.DeviceService.Interfaces;
public interface IDeviceService
{
    public Task ConnectDeviceAsync();
    public Task SendMessageAsync(string message);
    public void ChangeSendingAllowed(bool isAllowed);

    public bool IsAllowedToSend { get; }

    public event ConnectionStateChangedEventHandler ConnectionStateChangedEvent;
    public event SendingMessagesStateChangedEventHandler SendingMessagesStateChangedEvent;
}
