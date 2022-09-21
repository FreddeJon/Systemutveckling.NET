namespace Shared.Models.Entities;

public class DeviceSettingEntity
{
    public string DeviceId { get; set; } = Guid.NewGuid().ToString();
    public string? Owner { get; set; }
    public string? DeviceName { get; set; }
    public string? ConnectionString { get; set; }
}