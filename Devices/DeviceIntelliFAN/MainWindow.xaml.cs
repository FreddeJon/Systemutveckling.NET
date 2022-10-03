using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Application.Services.DeviceService.Events;
using Application.Services.DeviceService.Interfaces;

// ReSharper disable MemberCanBePrivate.Global

namespace DeviceIntelliFAN;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public sealed partial class MainWindow : INotifyPropertyChanged
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


        Closed += MainWindow_Closed;
    }


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
        _deviceService.ActionStateChangedEvent += DeviceService_SendingMessagesStateChangedEvent;
        _deviceService.ConnectionStateChangedEvent += DeviceService_ConnectionStateChangedEvent;

        _deviceService.ConnectDeviceAsync().ConfigureAwait(false);
    }

    private void ButtonToggleSendingState_OnClick(object sender, RoutedEventArgs e)
    {
        _deviceService.ChangeSendingAllowed(!IsAllowedToSend);

        Task.Run(async () =>
        {
            while (IsAllowedToSend)
            {
                await _deviceService.SendMessageAsync(data: new { message = "hej" });
            }
        }).ConfigureAwait(false);
    }

    private void DeviceService_ConnectionStateChangedEvent(object sender,
        ConnectionStateArgs e)
    {
        IsConnected = e.IsConnected;
        ConnectionStateMessage = e.Message;
    }

    private void DeviceService_SendingMessagesStateChangedEvent(object sender,
        SendingMessagesArgs e)
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
        _deviceService.ActionStateChangedEvent -= DeviceService_SendingMessagesStateChangedEvent;
        _deviceService.ConnectionStateChangedEvent -= DeviceService_ConnectionStateChangedEvent;

        Closed -= MainWindow_Closed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}