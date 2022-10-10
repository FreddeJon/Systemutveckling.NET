using Core.Services.DatabaseService;
using Core.Services.DatabaseService.Interfaces;
using Core.Services.DeviceService;
using Core.Services.DeviceService.Interfaces;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class RegisterDependencyExtension
{
    public static IServiceCollection AddDeviceService(this IServiceCollection services, DeviceSettings deviceSettings,
        DatabaseSettings? databaseSettings = null, ApiSettings? apiSettings = null)
    {
        apiSettings ??= new ApiSettings();
        databaseSettings ??= new DatabaseSettings();


        services.AddSingleton(databaseSettings);
        services.AddSingleton(deviceSettings);

        services.AddHttpClient<IDeviceService>("ConnectDevice",
            x => x.BaseAddress = new Uri(apiSettings.ApiBaseUrl + apiSettings.ConnectDeviceApiUrl));

        services.AddScoped<IDatabaseService, SqliteService>();
        services.AddScoped<IDeviceService, DeviceService>();
        return services;
    }
}