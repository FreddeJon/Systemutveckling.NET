using Core.Models;
using Newtonsoft.Json;

namespace Core.Services.WeatherService;

public class WeatherService : IWeatherService
{
    private const string ApiUrl =
        "https://api.openweathermap.org/data/2.5/weather?lat=59.1881139&lon=18.1140349&units=metric&appid=c67998ea6cc7bf6136954eb47ad2b6e8";

    public async Task<WeatherResponse> GetWeatherAsync()
    {

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(ApiUrl);

            if (!response.IsSuccessStatusCode)
                throw new ArgumentNullException();


            var data = JsonConvert.DeserializeObject<ApiWeatherResponse>(await response.Content.ReadAsStringAsync());

            if (data is null) throw new ArgumentNullException();

            if (data.main != null)
                return new WeatherResponse {Temperature = (int) data.main.temp, Humidity = data.main.humidity, WeatherCondition = data.weather[0].main};
        }
        catch
        {
            // ignored
        }

        return null!;
    }
}