namespace Application.Settings;
public class DeviceSettings
{
    public string DeviceId { get; init; } = Guid.NewGuid().ToString();
    public string DeviceOwner { get; init; } = "";
    public string DeviceName { get; init; } = "";
    public string DeviceType { get; init; } = "";
    public string DeviceLocation { get; init; } = "";
    public int Interval { get; set; } = 10000;
    public string ConnectionString { get; set; } = "";
}
