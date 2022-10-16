namespace Core.Models;

public class WeatherResponse
{
    private readonly int _temperature;
    public int Temperature
    {
        get => _temperature;
        init => _temperature = value;
    }
    public int Humidity { get; init; }

    private string? _weatherCondition;
    public string? WeatherCondition
    {
        get => _weatherCondition;
        set
        {
            switch (value?.ToLower())
            {
                case "clear":
                    _weatherCondition = "\ue28f";
                    break;

                case "clouds":
                    _weatherCondition = "\uf0c2";
                    break;

                case "rain":
                    _weatherCondition = "\uf740";
                    break;

                case "thunderstorm":
                    _weatherCondition = "\uf76c";
                    break;

                default:
                    _weatherCondition = "\uf6c4";
                    break;
            }
        }
    }
}