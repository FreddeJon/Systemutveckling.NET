using Core.Execeptions;
using Core.Models;
using Core.Services.DatabaseService.Interfaces;
using Core.Settings;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Core.Services.DeviceService.Helpers;
public class DeviceServiceInitializer
{
    public async Task<bool> GetConnectionStringFromApi(IDatabaseService sqliteService,
    DeviceSettings? deviceSettings,
    HttpClient client)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync("", new DeviceHttpRequest(deviceSettings?.DeviceId));

        var data = JsonConvert.DeserializeObject<DeviceHttpResponse>(await response.Content.ReadAsStringAsync());

        if (!response.IsSuccessStatusCode)
        {
            await Task.Delay(1000);
            return false;
        }

        deviceSettings!.ConnectionString =
            data?.ConnectionString ?? throw new ApiException("Error with getting connectionstring");
        await sqliteService.UpdateDeviceSettingsAsync(deviceSettings);

        return true;
    }

    public async Task<DeviceClient> InitializeClient(
        DeviceSettings? deviceSettings, DeviceClient? deviceClient)
    {
        try
        {
            if (deviceClient is not null)
            {
                await deviceClient.CloseAsync();
                await deviceClient.DisposeAsync();
            }

            deviceClient = DeviceClient.CreateFromConnectionString(deviceSettings?.ConnectionString,
                options: new ClientOptions { SdkAssignsMessageId = SdkAssignsMessageId.WhenUnset },
                transportType: TransportType.Amqp);

            if (deviceClient is null)
            {
                throw new DeviceClientException();
            }


            Twin? twin = await deviceClient.GetTwinAsync();

            try
            {
                deviceSettings!.Interval = twin.Properties.Desired["interval"];
            }
            catch
            {
                /* ignored */
            }
        }
        catch
        {
            throw new DeviceClientException("Restart client");
        }

        return deviceClient;
    }

    public async Task SetDeviceReportedDetails(DeviceSettings? deviceSettings, DeviceClient deviceClient)
    {
        var reported = new TwinCollection
        {
            ["deviceName"] = deviceSettings!.DeviceName,
            ["deviceType"] = deviceSettings.DeviceType,
            ["location"] = deviceSettings.Location?.ToLower(),
            ["owner"] = deviceSettings.Owner,

            ["actionState"] = "false"
        };
        await deviceClient.UpdateReportedPropertiesAsync(reported);
    }

}
