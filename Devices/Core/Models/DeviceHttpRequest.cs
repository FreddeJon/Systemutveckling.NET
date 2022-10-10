// ReSharper disable MemberCanBePrivate.Global
namespace Core.Models;
public class DeviceHttpRequest
{
    public DeviceHttpRequest(string? deviceId)
    {
        DeviceId = deviceId ?? throw new ArgumentNullException(deviceId);
    }
    public string DeviceId { get; }
}
