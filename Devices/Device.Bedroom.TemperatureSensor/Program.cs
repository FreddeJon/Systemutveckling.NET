using Core;
using Core.Execeptions;
using Core.Services.DeviceService.Events;
using Core.Services.DeviceService.Interfaces;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Bedroom.TemperatureSensor;

internal static class Program
{
    private static bool _isAllowed;
    private static bool _isConnected;
    private static IDeviceService? _deviceService;

    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddDeviceService(new DeviceSettings
        {
            DeviceName = "TemperatureSensor",
            DeviceType = "Sensor",
            Location = "Bedroom",
            Owner = "Fredrik"
        });

        ServiceProvider provider = services.BuildServiceProvider();

        _deviceService = provider.GetService<IDeviceService>() ?? throw new DeviceClientException();

        await Initializer();


        _deviceService.ChangeSendingAllowed(true);

        while (true)
        {
            if (_isConnected && _isAllowed)
            {
                await Task.Run(async () =>
                {
                    Console.WriteLine("Sending message");
                    // TODO SEND TEMP
                    await _deviceService.SendMessageAsync(new {Temperature = "24"});
                });
            }
        }
    }

    private static async Task Initializer()
    {
        _deviceService!.DeviceActionStateChangedEvent += OnDeviceActionStateChangedEvent;
        _deviceService.DeviceConnectionStateChangedEvent += OnConnectionStateChangedEvent;
        _deviceService.DeviceServiceErrorEvent += OnDeviceServiceErrorEvent;

        await _deviceService.ConnectDeviceAsync();
    }

    private static void OnDeviceServiceErrorEvent(object? sender, string e)
    {
        Console.WriteLine(e);
    }

    private static void OnConnectionStateChangedEvent(object? sender, ConnectionStateArgs e)
    {
        _isConnected = e.IsConnected;
        Console.WriteLine();
        Console.WriteLine(e.Message);
    }

    private static void OnDeviceActionStateChangedEvent(object? sender, SendingMessagesArgs e)
    {
        _isAllowed = e.IsAllowedToSend;
        Console.WriteLine();
        Console.WriteLine("Sending state: " + _isAllowed);
    }
}