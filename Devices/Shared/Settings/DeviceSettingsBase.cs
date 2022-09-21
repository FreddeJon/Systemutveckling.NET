namespace Shared.Settings;
public class DeviceSettingsBase
{
    public string DeviceId { get; init; } = Guid.NewGuid().ToString();
    public string? Owner { get; init; }
    public string? DeviceName { get; init; }
    public string? ConnectionString { get; init; }
}
