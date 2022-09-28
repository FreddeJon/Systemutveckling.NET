using Application.Services.DatabaseService;
using Application.Services.DatabaseService.Interfaces;
using Application.Services.DeviceService;
using Application.Services.DeviceService.Interfaces;
using Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class RegisterDependencyExtension
{
    public static IServiceCollection AddDatabaseSettings(this IServiceCollection services, DatabaseSettings settings)
    {
        services.AddSingleton<DatabaseSettings>(settings);
        return services;
    }

    public static IServiceCollection AddDeviceSetting(this IServiceCollection services, DeviceSettings settings)
    {
        services.AddSingleton<DeviceSettings>(settings);
        return services;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services, ApiSettings settings)
    {
        services.AddHttpClient<IDeviceService>("GetConnectionString", x => x.BaseAddress = new Uri(settings.ApiBaseUrl + settings.GetConnectionStringUrl));
        services.AddHttpClient<IDeviceService>("GetConnectionState", x => x.BaseAddress = new Uri(settings.ApiBaseUrl + settings.GetConnectionStateUrl));
        return services;
    }

    public static IServiceCollection AddDeviceService(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseService, SqliteService>();
        services.AddScoped<IDeviceService, DeviceService>();
        return services;
    }
}