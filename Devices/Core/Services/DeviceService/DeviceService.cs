using Core.Execeptions;
using Core.Models;
using Core.Services.DatabaseService.Interfaces;
using Core.Services.DeviceService.Events;
using Core.Services.DeviceService.Helpers;
using Core.Services.DeviceService.Interfaces;
using Core.Settings;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Text;

namespace Core.Services.DeviceService;

public sealed class DeviceService : IDeviceService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IDatabaseService _sqliteService;
    private ConnectionState _connectionState = ConnectionState.NotConnected;

#pragma warning disable CS0649
    private DeviceClient? _deviceClient;
#pragma warning restore CS0649
    private DeviceSettings? _deviceSettings;
    private bool _isAllowedToSendMessagesMessages;
    private readonly DeviceServiceInitializer _initializer;


    public DeviceService(IDatabaseService sqliteService, IHttpClientFactory clientFactory)
    {
        _sqliteService = sqliteService;
        _clientFactory = clientFactory;
        _initializer = new DeviceServiceInitializer();


        DeviceActionStateChangedEvent += OnDeviceActionStateChangedEvent;
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
            DeviceConnectionStateChangedEvent.Invoke(this,
                new ConnectionStateArgs(_connectionState.GetMessage(), IsConnected));
        }
    }

    public event EventHandler<ConnectionStateArgs> DeviceConnectionStateChangedEvent = delegate { };
    public event EventHandler<SendingMessagesArgs> DeviceActionStateChangedEvent = delegate { };
    public event EventHandler<string> DeviceServiceErrorEvent = delegate { };


    public bool IsAllowedToSendMessages
    {
        get => _isAllowedToSendMessagesMessages;
        private set
        {
            if (Equals(_isAllowedToSendMessagesMessages, value))
            {
                return;
            }

            _isAllowedToSendMessagesMessages = value;
            DeviceActionStateChangedEvent(this, new SendingMessagesArgs(_isAllowedToSendMessagesMessages));
        }
    }

    public async Task ConnectDeviceAsync()
    {
        try
        {
            _deviceSettings = await _sqliteService.GetDeviceSettingsAsync() ??
                              throw new DeviceSettingsException();

            await Task.Delay(100);

            ConnectionState = ConnectionState.Connecting;

            var isInitialized = !string.IsNullOrEmpty(_deviceSettings.ConnectionString);




            if (!isInitialized)
            {
                for (var i = 0; i <= 10; i++)
                {
                    ConnectionState = i < 5 ? ConnectionState.Connecting : ConnectionState.StillConnecting;
                    var successful = await _initializer.GetConnectionStringFromApi(_sqliteService, _deviceSettings,
                        _clientFactory.CreateClient("ConnectDevice"));

                    if (successful)
                    {
                        break;
                    }


                    if (i >= 10)
                    {
                        throw new ApiException("Could not connect");
                    }
                }
            }

            ConnectionState = ConnectionState.Initializing;
            await Task.Delay(100);
            _deviceClient = await _initializer.InitializeClient(_deviceSettings, _deviceClient);


            if (!isInitialized)
            {
                await _initializer.SetDeviceReportedDetails(_deviceSettings, _deviceClient ?? throw new DeviceClientException());
            }

            await _deviceClient!.SetMethodHandlerAsync("ChangeActionState", ChangeActionStateMethodHandler, null);
            await _deviceClient!.SetMethodHandlerAsync("UpdateDeviceProperties", UpdateDevicePropertiesMethodHandler,
                null);

            ConnectionState = ConnectionState.Connected;
        }
        catch (DeviceSettingsException e)
        {
            DeviceServiceErrorEvent.Invoke(this, e.Message);
        }
        catch (ApiException e)
        {
            DeviceServiceErrorEvent.Invoke(this, e.Message);
        }
        catch (DeviceClientException e)
        {
            DeviceServiceErrorEvent.Invoke(this, e.Message);
            await _sqliteService.ResetConnectionStringAsync(_deviceSettings);
        }
        catch (Exception)
        {
            // ignored
        }

        if (!IsConnected)
        {
            ConnectionState = ConnectionState.NotConnected;
        }
    }

    public async Task SendMessageAsync(object? data = null)
    {
        if (_deviceClient is null)
        {
            throw new DeviceClientException();
        }


        if (_deviceSettings is null)
        {
            throw new DeviceSettingsException();
        }


        if (IsAllowedToSendMessages)
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

    public bool ChangeSendingAllowed(bool isAllowed)
    {
        IsAllowedToSendMessages = IsConnected && isAllowed;

        return IsAllowedToSendMessages;
    }

    private Task<MethodResponse> ChangeActionStateMethodHandler(MethodRequest methodrequest, object usercontext)
    {
        var data = Encoding.UTF8.GetString(methodrequest.Data);
        var result = "{\"result\":\"Invalid parameter\"}";
        if (!bool.TryParse(data, out var sendingState))
        {
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        }

        ChangeSendingAllowed(sendingState);

        result = "{\"result\":\"Executed method\"}";
        return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
    }

    private async Task<MethodResponse> UpdateDevicePropertiesMethodHandler(MethodRequest methodrequest,
        object usercontext)
    {
        var result = JsonConvert.SerializeObject(new { result = $"could not update device: {_deviceSettings!.DeviceId}" });


        var json = Encoding.UTF8.GetString(methodrequest.Data);
        var data = JsonConvert.DeserializeObject<UpdateDeviceRequest>(json);

        if (data is null)
        {

            return await Task.FromResult(
                new MethodResponse(Encoding.UTF8.GetBytes(result),
                    400));
        }

        _deviceSettings!.DeviceName = data.DeviceName;
        _deviceSettings!.DeviceType = data.DeviceType;
        _deviceSettings!.Location = data.Location;
        _deviceSettings!.Owner = data.Owner;
        _deviceSettings!.Interval = data.Interval;

        await _sqliteService.UpdateDeviceSettingsAsync(_deviceSettings);


        var reported = new TwinCollection
        {
            ["deviceName"] = _deviceSettings!.DeviceName,
            ["deviceType"] = _deviceSettings.DeviceType,
            ["location"] = _deviceSettings.Location.ToLower(),
            ["owner"] = _deviceSettings.Owner,
            ["interval"] = _deviceSettings.Interval
        };
        await _deviceClient!.UpdateReportedPropertiesAsync(reported);


        result = JsonConvert.SerializeObject(new { result = $"updated device:: {_deviceSettings!.DeviceId}" });

        return await Task.FromResult(
            new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
    }

    private async void OnDeviceActionStateChangedEvent(object? sender, SendingMessagesArgs e)
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
}