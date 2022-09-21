using Microsoft.Extensions.DependencyInjection;
using Shared.Settings;

namespace Shared;

public static class RegisterDependencyExtension
{
    public static IServiceCollection AddDatabaseSetting(this IServiceCollection service, DatabaseSettings settings)
    {
        service.AddSingleton(settings);
        return service;

    }
}