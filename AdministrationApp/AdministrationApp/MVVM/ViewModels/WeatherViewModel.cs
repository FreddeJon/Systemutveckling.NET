using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Services.WeatherService;

namespace AdministrationApp.MVVM.ViewModels;

public class WeatherViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private int _humidity;
    private float _temperature;

    public WeatherViewModel(IWeatherService weatherService)
    {
        _weatherService = weatherService;

        UpdateTemperatureAsync().ConfigureAwait(false);
    }

    public int Humidity
    {
        get => _humidity;
        set => SetProperty(ref _humidity, value);
    }

    public float Temperature
    {
        get => _temperature;
        set => SetProperty(ref _temperature, value);
    }

    private async Task UpdateTemperatureAsync()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        var response = await _weatherService.GetWeatherAsync();
        Temperature = response.Temperature;
        Humidity = response.Humidity;

        while (await timer.WaitForNextTickAsync())
        {
            response = await _weatherService.GetWeatherAsync();
            Temperature = response.Temperature;
            Humidity = response.Humidity;
        }
    }
}