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
        _sqliteService = sqliteService;
        _clientFactory = clientFactory;

        SendingMessagesStateChangedEvent += DeviceService_SendingMessagesStateChangedEvent;
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


    public event ConnectionStateChangedEventHandler? ConnectionStateChangedEvent;
    public event SendingMessagesStateChangedEventHandler? SendingMessagesStateChangedEvent;

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
            SendingMessagesStateChangedEvent?.Invoke(this, new SendingMessagesArgs(_isAllowedToSend));
        }
    }

    public async Task ConnectDeviceAsync()
    {
        try
        {
            _deviceSettings = await _sqliteService.GetDeviceSettings() ??
                              throw new DeviceSettingsException();

            ConnectionState = ConnectionState.Connecting;

            var isInitialized = !string.IsNullOrEmpty(_deviceSettings.ConnectionString);

            if (!isInitialized)
            {
                for (var i = 0; i < 10; i++)
                {
                    ConnectionState = i < 5 ? ConnectionState.Connecting : ConnectionState.StillConnecting;

                    var successfull = await GetConnectionStringFromApi(_sqliteService, _deviceSettings,
                        _clientFactory.CreateClient("GetConnectionString"));

                    if (successfull)
                    {
                        break;
                    }

                    if (i >= 9)
                    {
                        throw new ApiException("could not connect");
                    }
                }
            }

            ConnectionState = ConnectionState.Initializing;

            await InitializeClient(_deviceSettings);

            if (_deviceClient is null)
            {
                throw new ArgumentNullException("");
            }


            Twin? twin = await _deviceClient.GetTwinAsync();

            try
            {
                _deviceSettings.Interval = twin.Properties.Desired["interval"];
            }
            catch
            {
                // ignored
            }

            if (!isInitialized)
            {
                await ReportDeviceDetails(_deviceSettings);
            }


            await _deviceClient.SetMethodHandlerAsync("ChangeSendingState", ChangeSendingStateMethodHandler, null);

            ConnectionState = ConnectionState.Connected;
        }
        catch (DeviceSettingsException e)
        {
            //TODO SET Error message
            throw;
        }
        catch (ApiException e)
        {
            //TODO SET Error message
        }
        catch
        {
            // ignored
        }

        if (!IsConnected)
        {
            ConnectionState = ConnectionState.NotConnected;
        }
    }


    public async Task SendMessageAsync(string message = null!)
    {
        if (_deviceClient is null)
        {
            return;
        }

        if (_deviceSettings is null)
        {
            throw new DeviceSettingsException();
        }


        if (IsAllowedToSend)
        {
            await _deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(DateTime.Now + " " + message)));
            await Task.Delay(_deviceSettings.Interval);
        }
    }


    /// <summary>
    ///     Sets allowed to true or false. Needs to be Connected
    /// </summary>
    /// <param name="isAllowed"></param>
    public void ChangeSendingAllowed(bool isAllowed)
    {
        IsAllowedToSend = IsConnected && isAllowed;
    }


    /// <summary>
    ///     Updates device twin with property running true/false depending on current IsAllowedToSend state
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DeviceService_SendingMessagesStateChangedEvent(object sender, SendingMessagesArgs e)
    {
        if (_deviceClient is null)
        {
            return;
        }

        await _deviceClient.UpdateReportedPropertiesAsync(new TwinCollection
        {
            ["running"] = e.IsAllowedToSend.ToString().ToLower()
        });
    }


    /// <summary>
    /// Method that recieves incomming request of changing sendingState
    /// </summary>
    /// <param name="methodrequest"></param>
    /// <param name="usercontext"></param>
    /// <returns></returns>
    private Task<MethodResponse> ChangeSendingStateMethodHandler(MethodRequest methodrequest, object usercontext)
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


    /// <summary>
    ///     Gets a connection string from API, throws error if not found
    /// </summary>
    /// <param name="sqliteService"></param>
    /// <param name="deviceSettings"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    /// <exception cref="DeviceSettingsException"></exception>
    /// <exception cref="ApiException"></exception>
    private static async Task<bool> GetConnectionStringFromApi(IDatabaseService sqliteService,
        DeviceSettings? deviceSettings,
        HttpClient client)
    {
        if (deviceSettings is null)
        {
            throw new DeviceSettingsException();
        }

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

        await sqliteService.UpdateDeviceSettings(deviceSettings);


        return true;
    }

    /// <summary>
    ///     Initializes the client
    /// </summary>
    /// <param name="createClient"></param>
    /// <param name="deviceSettings"></param>
    /// <returns></returns>
    /// <exception cref="DeviceSettingsException"></exception>
    private async Task InitializeClient(
        DeviceSettings? deviceSettings)
    {
        if (deviceSettings is null)
        {
            throw new DeviceSettingsException();
        }

        if (_deviceClient is not null)
        {
            try
            {
                await _deviceClient.CloseAsync();
            }
            catch
            {
                // ignored
            }

            await _deviceClient.DisposeAsync();
        }

        try
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(deviceSettings.ConnectionString,
                options: new ClientOptions {SdkAssignsMessageId = SdkAssignsMessageId.WhenUnset},
                transportType: TransportType.Amqp);

            Twin? twin = await _deviceClient.GetTwinAsync();

            try
            {
                deviceSettings.Interval = twin.Properties.Desired["interval"];
            }
            catch
            {
                // ignored
            }
        }
        catch
        {
            // ignored
        }
    }


    private async Task ReportDeviceDetails(DeviceSettings? deviceSettings)
    {
        if (deviceSettings is null)
        {
            throw new DeviceSettingsException();
        }


        var reported = new TwinCollection
        {
            ["deviceName"] = deviceSettings.DeviceName,
            ["deviceLocation"] = deviceSettings.DeviceLocation,
            ["deviceOwner"] = deviceSettings.DeviceOwner,
            ["deviceType"] = deviceSettings.DeviceType,
            ["running"] = "false"
        };
        await _deviceClient!.UpdateReportedPropertiesAsync(reported);
    }
}