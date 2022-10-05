using System.Net.Http.Json;
using System.Text;
using Application.Execeptions;
using Application.Models;
using Application.Services.DatabaseService.Interfaces;
using Application.Services.DeviceService.Events;
using Application.Services.DeviceService.Helpers;
using Application.Services.DeviceService.Interfaces;
using Application.Settings;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace Application.Services.DeviceService;

public class DeviceService : IDeviceService
{


    private readonly IHttpClientFactory _clientFactory;
    private readonly IDatabaseService _sqliteService;
    private ConnectionState _connectionState = ConnectionState.NotConnected;

    private DeviceClient? _deviceClient;
    private DeviceSettings? _deviceSettings;
    private bool _isAllowedToSend;

    public DeviceService(IDatabaseService sqliteService, IHttpClientFactory clientFactory)
    {
        _sqliteService = sqliteService;
        _clientFactory = clientFactory;

        ActionStateChangedEvent += DeviceService_ActionStateChangedEvent;
    }

    private bool IsConnected => ConnectionState == ConnectionState.Connected;

    private ConnectionState ConnectionState
    {
        get => _connectionState;
        set
        {
            if (Equals(_connectionState, value))
            {
                return;
            }

            _connectionState = value;
            ConnectionStateChangedEvent?.Invoke(this,
                new ConnectionStateArgs(_connectionState.GetMessage(), IsConnected));
        }
    }


    public event ConnectionStateChangesEventHandler? ConnectionStateChangedEvent;
    public event ActionStateChangesEventHandler? ActionStateChangedEvent;

    public bool IsAllowedToSend
    {
        get => _isAllowedToSend;
        private set
        {
            if (Equals(_isAllowedToSend, value))
            {
                return;
            }

            _isAllowedToSend = value;
            ActionStateChangedEvent?.Invoke(this, new SendingMessagesArgs(_isAllowedToSend));
        }
    }

    public async Task ConnectDeviceAsync()
    {
        try
        {
            _deviceSettings = await _sqliteService.GetDeviceSettingsAsync() ??
                              throw new DeviceSettingsException();

            ConnectionState = ConnectionState.Connecting;

            var isInitialized = !string.IsNullOrEmpty(_deviceSettings.ConnectionString);

            if (!isInitialized)
            {
                for (var i = 0; i <= 10; i++)
                {
                    ConnectionState = i < 5 ? ConnectionState.Connecting : ConnectionState.StillConnecting;

                    var successfull = await GetConnectionStringFromApi(_sqliteService, _deviceSettings,
                        _clientFactory.CreateClient("ConnectDevice"));

                    if (successfull)
                        break;


                    if (i >= 10)
                        throw new ApiException("could not connect");
                }
            }

            ConnectionState = ConnectionState.Initializing;

            await InitializeClient(_deviceSettings);


            if (!isInitialized)
                await SetDeviceReportedDetails(_deviceSettings);

            await _deviceClient!.SetMethodHandlerAsync("ChangeActionState", ChangeActionStateMethodHandler, null);
            await _deviceClient!.SetMethodHandlerAsync("UpdateDeviceProperties", UpdateDevicePropertiesMethodHandler, null);

            ConnectionState = ConnectionState.Connected;
        }
        catch (DeviceSettingsException e)
        {
            //TODO SET Error message ERROR WITH DEVICESETTINGS
        }
        catch (ApiException e)
        {
            //TODO SET Error message API ERROR
        }
        catch (DeviceClientException e)
        {
            //TODO SET Error message RESTART DEVICE
            await _sqliteService.ResetConnectionStringAsync(_deviceSettings);
        }
        catch (Exception e)
        {
            // ignored
        }

        if (!IsConnected)
            ConnectionState = ConnectionState.NotConnected;
    }



    public async Task SendMessageAsync(object? data = null)
    {
        if (_deviceClient is null)
            throw new DeviceClientException();


        if (_deviceSettings is null)
            throw new DeviceSettingsException();



        if (IsAllowedToSend)
        {
            var json = JsonConvert.SerializeObject(data);

            var message = new Message(Encoding.UTF8.GetBytes(json));
            message.Properties.Add("deviceName", _deviceSettings.DeviceName);
            message.Properties.Add("deviceType", _deviceSettings.DeviceType);
            message.Properties.Add("location", _deviceSettings.Location);
            message.Properties.Add("owner", _deviceSettings.Owner);
            await _deviceClient.SendEventAsync(message);

            await Task.Delay(_deviceSettings.Interval);
        }
    }

    public void ChangeSendingAllowed(bool isAllowed)
    {
        IsAllowedToSend = IsConnected && isAllowed;
    }


    private async void DeviceService_ActionStateChangedEvent(object sender, SendingMessagesArgs e)
    {
        if (_deviceClient is null)
        {
            return;
        }

        await _deviceClient.UpdateReportedPropertiesAsync(new TwinCollection
        {
            ["actionState"] = e.IsAllowedToSend.ToString().ToLower()
        });
    }

    private Task<MethodResponse> ChangeActionStateMethodHandler(MethodRequest methodrequest, object usercontext)
    {
        var data = Encoding.UTF8.GetString(methodrequest.Data);

        if (bool.TryParse(data, out var sendingState))
        {
            ChangeSendingAllowed(sendingState);
            // Acknowlege the direct method call with a 200 success message.
            var result = $"{{\"result\":\"Executed direct method: {methodrequest.Name}\"}}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }
        else
        {
            // Acknowlege the direct method call with a 400 error message.
            var result = "{\"result\":\"Invalid parameter\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        }
    }
    private async Task<MethodResponse> UpdateDevicePropertiesMethodHandler(MethodRequest methodrequest, object usercontext)
    {
        var result = $"{{\"result\":\"Executed direct method: {methodrequest.Name}\"}}";
        var json = Encoding.UTF8.GetString(methodrequest.Data);

        var data = JsonConvert.DeserializeObject<UpdateDeviceRequest>(json);

        if (data is null)
        {
            return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        }

        _deviceSettings!.DeviceName = data.DeviceName;
        _deviceSettings!.DeviceType = data.DeviceType;
        _deviceSettings!.Location = data.Location;
        _deviceSettings!.Owner = data.Owner;
        _deviceSettings!.Interval= data.Interval;



        await _sqliteService.UpdateDeviceSettingsAsync(_deviceSettings);

        var reported = new TwinCollection
        {
            ["deviceName"] = _deviceSettings!.DeviceName,
            ["deviceType"] = _deviceSettings.DeviceType,
            ["location"] = _deviceSettings.Location.ToLower(),
            ["owner"] = _deviceSettings.Owner,
            ["interval"] = _deviceSettings.Interval,
        };
        await _deviceClient!.UpdateReportedPropertiesAsync(reported);

        return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
    }


    private static async Task<bool> GetConnectionStringFromApi(IDatabaseService sqliteService,
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
            data?.ConnectionString ?? throw new ApiException("error with getting connectionstring");

        await sqliteService.UpdateDeviceSettingsAsync(deviceSettings);

        return true;
    }

    private async Task InitializeClient(
        DeviceSettings? deviceSettings)
    {

        try
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(deviceSettings?.ConnectionString,
                options: new ClientOptions { SdkAssignsMessageId = SdkAssignsMessageId.WhenUnset },
                transportType: TransportType.Amqp);

            if (_deviceClient is null)
                throw new DeviceClientException();



            Twin? twin = await _deviceClient.GetTwinAsync();

            try { deviceSettings!.Interval = twin.Properties.Desired["interval"]; } catch { /* ignored */ }
        }
        catch
        {
            throw new DeviceClientException("restart client");
        }
    }

    private async Task SetDeviceReportedDetails(DeviceSettings? deviceSettings)
    {
        var reported = new TwinCollection
        {
            ["deviceName"] = deviceSettings!.DeviceName,
            ["deviceType"] = deviceSettings.DeviceType,
            ["location"] = deviceSettings.Location.ToLower(),
            ["owner"] = deviceSettings.Owner,

            ["actionState"] = "false"
        };
        await _deviceClient!.UpdateReportedPropertiesAsync(reported);
    }
}