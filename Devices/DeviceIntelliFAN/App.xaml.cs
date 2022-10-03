using System.Windows;
using Application;
using Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceIntelliFAN;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        _serviceProvider = ConfigureServices(new ServiceCollection()).BuildServiceProvider();
    }

    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<MainWindow>();
        services.AddDatabaseSettings(new DatabaseSettings());
        services.AddDeviceSetting(new DeviceSettings()
        {
            DeviceName = "IntelliFAN",
            Location = "Kitchen",
            DeviceType = "Fan",
            Owner = "Fredrik"
        });

        services.AddHttpClients(new ApiSettings(
            apiBaseUrl: "https://systemutveckling-kyh.azurewebsites.net/api/devices/connect",
            connectDeviceApiUrl: "?code=eCVbmfhXXdnSDoFxRNvpzOjowUnXwAqicmuqVsAtysYqAzFuQ3NFkQ=="
        ));

        services.AddDeviceService();

        return services;
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainWindow = _serviceProvider.GetService<MainWindow>();

        mainWindow?.Show();
    }
}