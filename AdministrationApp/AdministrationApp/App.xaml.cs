using AdministrationApp.Helpers.AutoMapper;
using AdministrationApp.MVVM.ViewModels;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

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

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<KitchenViewModel>();
        services.AddTransient<BedroomViewModel>();
        services.AddTransient<DeviceListViewModel>();
        services.AddTransient<EditDeviceViewModel>();


        return services.BuildServiceProvider();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetService<MainWindow>();


        mainWindow?.Show();
    }
}