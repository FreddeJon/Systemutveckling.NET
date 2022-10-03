using Microsoft.Azure.Devices;
using System;

namespace AzureFunctions.Models;

public class DeviceHttpResponse
{

    public DeviceHttpResponse(string message)
    {
        Message = message;
    }

    public DeviceHttpResponse(Device device)
    {
        ConnectionString =
            $"HostName={Environment.GetEnvironmentVariable("IotHub")?.Split(";")[0].Split("=")[1]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
    }


    public string Message { get; } = "device connected successfully";
    public string? ConnectionString { get; }

}