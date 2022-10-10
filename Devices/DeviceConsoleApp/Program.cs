using Core;
using Core.Execeptions;
using Core.Services.DeviceService.Events;
using Core.Services.DeviceService.Interfaces;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDeviceService(new DeviceSettings()
{
    DeviceName = "Lamp",
    DeviceType = "Light",
    Location = "kitchen",
    Owner = "Fredrik"
});


var provider = services.BuildServiceProvider();

var device = provider.GetService<IDeviceService>();


if (device is null)
    throw new DeviceClientException();


await device.ConnectDeviceAsync();
device.DeviceActionStateChangedEvent += DeviceSendingMessagesStateChangedEvent;


device.ChangeSendingAllowed(true);
while (true)
{
    if (device.IsAllowedToSendMessages)
    {
        await Task.Run(async () =>
        {
            var message = await GetTemp();
            Console.WriteLine("sending: " + message);
            await device.SendMessageAsync(message);
        });
    }
}


void DeviceSendingMessagesStateChangedEvent(object? sender, SendingMessagesArgs e)
{
    Console.WriteLine(e.IsAllowedToSend + " FROM EVENT");
}



async Task<string> GetTemp()
{
    await Task.Delay(100);


    var rnad = new Random();

    return $"Temp: {rnad.Next(20, 30)}, humidity: {rnad.Next(30, 40)}";
}