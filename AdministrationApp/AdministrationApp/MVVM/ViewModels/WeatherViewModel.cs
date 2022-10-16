using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Services.WeatherService;
// ReSharper disable MemberCanBePrivate.Global

namespace AdministrationApp.MVVM.ViewModels;

public class WeatherViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private int _humidity;
    private float _temperature;
    private string? _weatherCondition;

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

    public string? WeatherCondition
    {
        get => _weatherCondition;
        set => SetProperty(ref _weatherCondition, value);
    }

    private async Task UpdateTemperatureAsync()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        var response = await _weatherService.GetWeatherAsync();
        Temperature = response.Temperature;
        Humidity = response.Humidity;
        WeatherCondition = response.WeatherCondition;

        while (await timer.WaitForNextTickAsync())
        {
            response = await _weatherService.GetWeatherAsync();
            Temperature = response.Temperature;
            Humidity = response.Humidity;
            WeatherCondition = response.WeatherCondition;
        }
    }

}