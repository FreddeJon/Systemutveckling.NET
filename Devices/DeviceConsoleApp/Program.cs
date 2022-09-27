// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Services.DatabaseService;
using Shared.Services.DatabaseService.Interfaces;
using Shared.Services.DeviceService;
using Shared.Services.DeviceService.Interfaces;
using Shared.Settings;

var services = new ServiceCollection();

services.AddDatabaseSettings(new DatabaseSettings(databaseName: "TEST"));
services.AddDeviceSetting(new DeviceSettings()
{
    DeviceId = "he",
    DeviceLocation = "Test",
    DeviceName = "ConsoleApp",
    DeviceType = "Console",
    DeviceOwner = "Fredriks"

});

services.AddHttpClients(new ApiSettings(
    apiBaseUrl: "https://systemutveckling-kyh.azurewebsites.net/api/devices/connect",
    getConnectionStringUrl: "?code=eCVbmfhXXdnSDoFxRNvpzOjowUnXwAqicmuqVsAtysYqAzFuQ3NFkQ==",
    getConnectionStateUrl: "?code=ke798xgRCqRRD1SFmx9F_nMCrHXQ_owPtcsD9qsbjvq6AzFu8wFm9Q==&deviceId="
));

services.AddDeviceService();


var provider = services.BuildServiceProvider();

var device = provider.GetService<IDeviceService>();


if (device != null)
{
    await device.ConnectDeviceAsync();
}


device.SendingMessagesStateChangedEvent += DeviceSendingMessagesStateChangedEvent;



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


void DeviceSendingMessagesStateChangedEvent(object sender, Shared.Services.DeviceService.Events.SendingMessagesArgs e)
{
    Console.WriteLine(e.IsAllowedToSend + " FROM EVENT");
}



async Task<string> GetTemp()
{
    await Task.Delay(100);


    var rnad = new Random();

    return $"Temp: {rnad.Next(20, 30)}, humidity: {rnad.Next(30,40)}";
}