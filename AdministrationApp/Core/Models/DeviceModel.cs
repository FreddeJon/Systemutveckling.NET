namespace Core.Models;

public class DeviceModel
{
    public string DeviceId { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public string DeviceType { get; set; } = "Unknown";

    public bool ActionState { get; set; }

    public string IconActiveState { get; set; } = "";
    public string IconInActiveState { get; set; } = "";
    public string TextActiveState { get; set; } = "";
    public string TextInActiveState { get; set; } = "";
    public string? ConnectionState { get; set; }
    public DateTime? LastActivityTime { get; set; }
    public string Location { get; set; } = "Unknown";
    public string Owner { get; set; } = "Unkown";
    public int Interval { get; set; } = 10000;
}