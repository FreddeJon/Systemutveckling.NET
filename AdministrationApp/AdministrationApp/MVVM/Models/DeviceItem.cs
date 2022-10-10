// ReSharper disable ClassNeverInstantiated.Global

namespace AdministrationApp.MVVM.Models;

public class DeviceItem : ObservableObject
{
    private bool _actionState;
    private bool _connectionState;
    private string _deviceId = "";
    private string _deviceName = "";
    private string _deviceType = "Unknown";
    private string _iconActiveState = "";
    private string _iconInActiveState = "";
    private int _interval;
    private string _location = "Unkown";
    private string _owner = "Unkown";
    private string _textActiveState = "";
    private string _textInActiveState = "";

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

    public bool ConnectionState
    {
        get => _connectionState;
        set
        {
            if (Equals(_connectionState, value)) return;
            SetProperty(ref _connectionState, value);
        }
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