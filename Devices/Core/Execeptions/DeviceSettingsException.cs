namespace Core.Execeptions
{
    public class DeviceSettingsException : Exception
    {
        public DeviceSettingsException() : base("Could not load device settings")
        {
            
        }
        public DeviceSettingsException(string message) : base(message)
        {
            
        }
    }
}
