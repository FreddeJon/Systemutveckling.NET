// See https://aka.ms/new-console-template for more information


using Application;
using Application.Execeptions;
using Application.Services.DeviceService.Events;
using Application.Services.DeviceService.Interfaces;
using Application.Settings;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDatabaseSettings(new DatabaseSettings(databaseName: "TEST"));
services.AddDeviceSetting(new DeviceSettings()
{
    DeviceName = "Lamp",
    DeviceType = "Light",
    Location = "kitchen",
    Owner = "Fredrik"

});

services.AddHttpClients(new ApiSettings(
    apiBaseUrl: "https://systemutveckling-kyh.azurewebsites.net/api/devices/connect",
    connectDeviceApiUrl: "?code=eCVbmfhXXdnSDoFxRNvpzOjowUnXwAqicmuqVsAtysYqAzFuQ3NFkQ=="
));

services.AddDeviceService();


var provider = services.BuildServiceProvider();

var device = provider.GetService<IDeviceService>();


if (device is null)
    throw new DeviceClientException();


await device.ConnectDeviceAsync();
device.ActionStateChangedEvent += DeviceSendingMessagesStateChangedEvent;


device.ChangeSendingAllowed(true);
while (true)
{
    if (device.IsAllowedToSend)
    {
        await Task.Run(async () =>
        {
            var message = await GetTemp();
            Console.WriteLine("sending: " + message);
            await device.SendMessageAsync(message);
        });
    }
}


void DeviceSendingMessagesStateChangedEvent(object sender, SendingMessagesArgs e)
{
    Console.WriteLine(e.IsAllowedToSend + " FROM EVENT");
}



async Task<string> GetTemp()
{
    await Task.Delay(100);


    var rnad = new Random();

    return $"Temp: {rnad.Next(20, 30)}, humidity: {rnad.Next(30, 40)}";
}