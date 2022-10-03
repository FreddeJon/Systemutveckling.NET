using Application.Services.DeviceService.Events;

namespace Application.Services.DeviceService.Interfaces;
public interface IDeviceService
{
    public Task ConnectDeviceAsync();
    public Task SendMessageAsync(object data);
    public void ChangeSendingAllowed(bool isAllowed);

    public bool IsAllowedToSend { get; }

    public event ConnectionStateChangesEventHandler ConnectionStateChangedEvent;
    public event ActionStateChangesEventHandler ActionStateChangedEvent;
}
