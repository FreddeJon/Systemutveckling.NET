using Shared.Services.DeviceService.Events;

namespace Shared.Services.DeviceService.Interfaces;
public interface IDeviceService
{
    public Task ConnectDeviceAsync();
    public Task SendMessagesAsync();

    public event ConnectionStateChangedEventHandler ConnectionStateChangedEvent;
    public event AllowSendMessagesChangedEventHandler AllowSendMessagesChangedEvent;
}
