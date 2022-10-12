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


            var data = JsonConvert.DeserializeObject<Rootobject>(await response.Content.ReadAsStringAsync());

            if (data is null) throw new ArgumentNullException();

            if (data.main != null)
                return new WeatherResponse {Temperature = data.main.temp, Humidity = data.main.humidity};
        }
        catch
        {
            // ignored
        }

        return null!;
    }
}

public class Rootobject
{
    public Coord coord { get; set; }
    public Weather[] weather { get; set; }
    public string _base { get; set; }
    public Main main { get; set; }
    public int visibility { get; set; }
    public Wind wind { get; set; }
    public Clouds clouds { get; set; }
    public int dt { get; set; }
    public Sys sys { get; set; }
    public int timezone { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int cod { get; set; }
}

public class Coord
{
    public float lon { get; set; }
    public float lat { get; set; }
}

public class Main
{
    public float temp { get; set; }
    public float feels_like { get; set; }
    public float temp_min { get; set; }
    public float temp_max { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
}

public class Wind
{
    public float speed { get; set; }
    public int deg { get; set; }
    public float gust { get; set; }
}

public class Clouds
{
    public int all { get; set; }
}

public class Sys
{
    public int type { get; set; }
    public int id { get; set; }
    public string country { get; set; }
    public int sunrise { get; set; }
    public int sunset { get; set; }
}

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}