namespace Shared.Settings
{
    public class ApiSettings
    {
        public string ApiBaseUrl { get; }
        public string GetConnectionStateUrl { get; }
        public string GetConnectionStringUrl { get; }

        public ApiSettings(string apiBaseUrl, string getConnectionStateUrl, string getConnectionStringUrl)
        {
            ApiBaseUrl = apiBaseUrl;
            GetConnectionStateUrl = getConnectionStateUrl;
            GetConnectionStringUrl = getConnectionStringUrl;
        }
    }
}
