namespace Application.Settings;
public class DeviceSettings
{
    public string DeviceId { get; init; } = Guid.NewGuid().ToString();
    public string DeviceName { get; init; } = "";
    public string DeviceType { get; init; } = "";
    public string Owner { get; init; } = "";
    public string Location { get; init; } = "";
    public int Interval { get; set; } = 10000;
    public string ConnectionString { get; set; } = "";
}
