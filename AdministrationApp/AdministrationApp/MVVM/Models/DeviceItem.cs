namespace AdministrationApp.MVVM.Models;

public class DeviceItem : ObservableObject
{
    private string _deviceId = "";
    private string _deviceName = "";
    private string _deviceType = "Unknown";
    private bool _actionState;
    private string _iconActiveState = "";
    private string _iconInActiveState = "";
    private string _textActiveState = "";
    private string _textInActiveState = "";
    private int _interval;
    private string _location = "Unkown";
    private string _owner = "Unkown";
    private string _connectionState = "Disconnected";

    public string DeviceId
    {
        get => _deviceId;
        set => SetProperty(ref _deviceId, value);
    }

    public string DeviceName
    {
        get => _deviceName;
        set => SetProperty(ref _deviceName, value);
    }

    public string DeviceType
    {
        get => _deviceType;
        set => SetProperty(ref _deviceType, value);
    }

    public string ConnectionState
    {
        get => _connectionState;
        set => SetProperty(ref _connectionState, value);
    }

    public bool ActionState
    {
        get => _actionState;
        set => SetProperty(ref _actionState, value);
    }

    public string IconActiveState
    {
        get => _iconActiveState;
        set => SetProperty(ref _iconActiveState, value);
    }

    public string IconInActiveState
    {
        get => _iconInActiveState;
        set => SetProperty(ref _iconInActiveState, value);
    }

    public string TextActiveState
    {
        get => _textActiveState;
        set => SetProperty(ref _textActiveState, value);
    }

    public string TextInActiveState
    {
        get => _textInActiveState;
        set => SetProperty(ref _textInActiveState, value);
    }

    public string Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    public string Owner
    {
        get => _owner;
        set => SetProperty(ref _owner, value);
    }

    public int Interval
    {
        get => _interval;
        set => SetProperty(ref _interval, value);
    }
}