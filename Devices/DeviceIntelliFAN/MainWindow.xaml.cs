using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DeviceIntelliFAN.Core;
using System.Windows.Media.Animation;
using Shared.Services.DeviceService.Interfaces;
// ReSharper disable MemberCanBePrivate.Global

namespace DeviceIntelliFAN;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly IDeviceService _deviceService;
    private bool _isConnected;
    private string _connectionStateMessage = "Connecting";
    private bool _isAllowedToSend;
    private string _toggleSendingStateButton = "Start";


    public MainWindow(IDeviceService deviceService)
    {
        _deviceService = deviceService;


        InitializeComponent();
        this.DataContext = this;

        Initialize();

        ToggleSendingStateCommand = new DelegateCommand(ToggleSendingState);

        Closed += MainWindow_Closed;
    }


    public DelegateCommand ToggleSendingStateCommand { get; set; }

    public bool IsAllowedToSend
    {
        get => _isAllowedToSend;
        set
        {
            if (value == _isAllowedToSend) return;
            _isAllowedToSend = value;
            OnPropertyChanged();
        }
    }

    public string ConnectionStateMessage
    {
        get => _connectionStateMessage;
        set
        {
            if (value == _connectionStateMessage) return;
            _connectionStateMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (value == _isConnected) return;
            _isConnected = value;
            OnPropertyChanged();
        }
    }

    public string ToggleSendingStateButton
    {
        get => _toggleSendingStateButton;
        set
        {
            if (value == _toggleSendingStateButton) return;
            _toggleSendingStateButton = value;
            OnPropertyChanged();
        }
    }

    private void Initialize()
    {
        _deviceService.SendingMessagesStateChangedEvent += DeviceService_SendingMessagesStateChangedEvent;
        _deviceService.ConnectionStateChangedEvent += DeviceService_ConnectionStateChangedEvent;


        _deviceService.ConnectDeviceAsync().ConfigureAwait(false);
    }

    private void ToggleSendingState(object? obj)
    {
        _deviceService.ChangeSendingAllowed(!IsAllowedToSend);

        Task.Run(async () =>
        {
            while (IsAllowedToSend)
            {
                await _deviceService.SendMessageAsync(message: "Hey");
            }
        }).ConfigureAwait(false);
    }


    private void DeviceService_ConnectionStateChangedEvent(object sender,
        Shared.Services.DeviceService.Events.ConnectionStateArgs e)
    {
        IsConnected = e.IsConnected;
        ConnectionStateMessage = e.Message;
    }


    private void DeviceService_SendingMessagesStateChangedEvent(object sender,
        Shared.Services.DeviceService.Events.SendingMessagesArgs e)
    {
        IsAllowedToSend = e.IsAllowedToSend;

        Dispatcher.Invoke(() =>
        {
            ToggleSendingStateButton = IsAllowedToSend ? "Stop" : "Start";

            var sb = (BeginStoryboard) TryFindResource("SbRotate");
            if (IsAllowedToSend)
            {
                sb.Storyboard.Begin();
            }
            else
            {
                sb.Storyboard.Stop();
            }
        });
    }


    private void MainWindow_Closed(object? sender, System.EventArgs e)
    {
        _deviceService.SendingMessagesStateChangedEvent -= DeviceService_SendingMessagesStateChangedEvent;
        _deviceService.ConnectionStateChangedEvent -= DeviceService_ConnectionStateChangedEvent;

        Closed -= MainWindow_Closed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}