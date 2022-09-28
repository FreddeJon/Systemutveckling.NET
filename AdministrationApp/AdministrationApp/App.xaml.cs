using System;
using System.Windows;
using AdministrationApp.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AdministrationApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        _serviceProvider = ConfigureServices(new ServiceCollection());

    }

    private IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<MainWindow>();
        services.AddScoped<MainViewModel>();
        services.AddScoped<KitchenViewModel>();

        return services.BuildServiceProvider();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetService<MainWindow>();

        
        mainWindow?.Show();
    }
}