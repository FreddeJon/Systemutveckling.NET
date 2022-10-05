using Microsoft.Azure.Devices;

namespace Core.Models
{
    public class DeviceModel
    {
        public string DeviceId { get; set; } = "";
        public string DeviceName { get; set; } = "";
        public string DeviceType { get; set; } = "Unknown";

        public bool DeviceActionState { get; set; }

        public string IconActiveState { get; set; } = "";
        public string IconInActiveState { get; set; } = "";
        public string TextActiveState { get; set; } = "";
        public string TextInActiveState { get; set; } = "";
        public DeviceConnectionState? ConnectionState { get; set; }
        public DateTime? LastActivityTime { get; set; }
    }
}
