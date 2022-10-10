namespace Core.Settings
{
    public class ApiSettings
    {
        public string ApiBaseUrl { get; } = "https://systemutveckling-kyh.azurewebsites.net/api/devices/connect";
        public string ConnectDeviceApiUrl { get; } = "?code=eCVbmfhXXdnSDoFxRNvpzOjowUnXwAqicmuqVsAtysYqAzFuQ3NFkQ==";

        public ApiSettings()
        {

        }

        public ApiSettings(string apiBaseUrl, string connectDeviceApiUrl)
        {
            ApiBaseUrl = apiBaseUrl;
            ConnectDeviceApiUrl = connectDeviceApiUrl;
        }
    }
}
