using System;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace AzureFunctions;

public static class ConnectDevice
{
    private static readonly RegistryManager RegistryManager =
        RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

    [FunctionName("ConnectDevice")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "devices/connect")]
        HttpRequest req)
    {
        try
        {
            var request =
                JsonConvert.DeserializeObject<DeviceHttpRequest>(await new StreamReader(req.Body).ReadToEndAsync());


            if (string.IsNullOrEmpty(request?.DeviceId))
                return new BadRequestObjectResult(new DeviceHttpResponse(message: "deviceId is required"));


            var device = await RegistryManager.GetDeviceAsync(request.DeviceId) ??
                         await CreateDevice(request.DeviceId);

            return new OkObjectResult(new DeviceHttpResponse(device));
        }
        catch
        {
            // ignored
        }


        return new BadRequestObjectResult(new DeviceHttpResponse(message: "could not connect the device"));
    }


    private static async Task<Device> CreateDevice(string deviceId)
    {
        var device = await RegistryManager.AddDeviceAsync(new Device(deviceId));

        if (device is null)
            throw new ArgumentNullException(nameof(device));

        await UpdateDesiredProperties(device.Id);
        return device;
    }

    private static async Task UpdateDesiredProperties(string deviceId)
    {
        var twin = await RegistryManager.GetTwinAsync(deviceId);

        twin.Properties.Desired["interval"] = 10000;

        await RegistryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
    }
}