using Core;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DeviceIntelliFAN;
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
        services.AddDeviceService(new DeviceSettings()
        {
            DeviceName = "IntelliFAN",
            Location = "Kitchen",
            DeviceType = "Fan",
            Owner = "Fredrik"
        });

        return services;
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainWindow = _serviceProvider.GetService<MainWindow>();

        mainWindow?.Show();
    }
}