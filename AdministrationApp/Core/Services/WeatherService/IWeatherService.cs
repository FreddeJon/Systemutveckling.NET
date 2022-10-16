using Core.Models;

namespace Core.Services.WeatherService;

public interface IWeatherService
{
    public Task<WeatherResponse> GetWeatherAsync();
}