using AdministrationApp.Helpers.AutoMapper;
using AdministrationApp.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Core.Services.WeatherService;
using Core.Services.DeviceService;

namespace AdministrationApp;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        _serviceProvider = ConfigureServices();
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Windows
        services.AddTransient<MainWindow>();

        // Services
        services.AddAutoMapper(typeof(MapperProfile));
        services.AddScoped<IDeviceManager, DeviceManager>();
        services.AddScoped<IWeatherService, WeatherService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<KitchenViewModel>();
        services.AddTransient<BedroomViewModel>();
        services.AddTransient<DeviceListViewModel>();
        services.AddTransient<EditDeviceViewModel>();
        services.AddSingleton<WeatherViewModel>();


        return services.BuildServiceProvider();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetService<MainWindow>();


        mainWindow?.Show();
    }
}