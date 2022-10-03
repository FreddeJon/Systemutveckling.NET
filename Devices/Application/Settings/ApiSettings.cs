namespace Application.Settings
{
    public class ApiSettings
    {
        public string ApiBaseUrl { get; }
        public string ConnectDeviceAPIUrl { get; }

        public ApiSettings(string apiBaseUrl, string connectDeviceApiUrl)
        {
            ApiBaseUrl = apiBaseUrl;
            ConnectDeviceAPIUrl = connectDeviceApiUrl;
        }
    }
}
