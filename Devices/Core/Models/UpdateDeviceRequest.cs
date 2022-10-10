namespace Core.Models;
public class UpdateDeviceRequest
{
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? Location { get; set; }
    public string? Owner { get; set; }
    public int Interval { get; set; }
}
