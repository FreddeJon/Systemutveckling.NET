namespace Core.Settings;
public class DeviceSettings
{
    public string DeviceId { get; set; } = Guid.NewGuid().ToString();
    public string? DeviceName { get; set; } = "";
    public string? DeviceType { get; set; } = "";
    public string? Owner { get; set; } = "";
    public string? Location { get; set; } = "";
    public int Interval { get; set; } = 10000;
    public string ConnectionString { get; set; } = "";
}
