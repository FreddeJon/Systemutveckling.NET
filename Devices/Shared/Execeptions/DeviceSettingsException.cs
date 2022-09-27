namespace Shared.Execeptions
{
    public class DeviceSettingsException : Exception
    {
        public DeviceSettingsException() : base("could not load device settings")
        {
            
        }
        public DeviceSettingsException(string message) : base(message)
        {
            
        }
    }
}
