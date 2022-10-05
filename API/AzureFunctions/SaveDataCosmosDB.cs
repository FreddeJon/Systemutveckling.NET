using System;
using System.Globalization;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Newtonsoft.Json;

namespace AzureFunctions;

public class SaveDataCosmosDb
{
    [FunctionName("SaveDataCosmosDB")]
    public void Run(
        [IoTHubTrigger("messages/events", Connection = "IotHubEndpoint", ConsumerGroup = "azurefunction")]
        EventData message,
        [CosmosDB(databaseName: "Devices.NET", collectionName: "Data", CreateIfNotExists = true,
            ConnectionStringSetting = "CosmosDB")]
        out dynamic cosmos)
    {
        try
        {
            if (message.Body.Array is null)
                throw new ArgumentNullException(nameof(message));

            cosmos = new
            {
                deviceId = message.SystemProperties["iothub-connection-device-id"].ToString(),
                deviceName = message.Properties["deviceName"].ToString(),
                deviceType = message.Properties["deviceType"].ToString(),
                location = message.Properties["location"].ToString(),
                owner = message.Properties["owner"].ToString(),
                date = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                data = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(message.Body.Array))
            };
        }
        catch
        {
            cosmos = null!;
        }
    }
}