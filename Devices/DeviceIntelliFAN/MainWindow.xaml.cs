using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Core.Services.DeviceService.Events;
using Core.Services.DeviceService.Interfaces;

// ReSharper disable MemberCanBePrivate.Global

namespace DeviceIntelliFAN;

public sealed partial class MainWindow : INotifyPropertyChanged
{
    private readonly IDeviceService _deviceService;
    private string _connectionStateMessage = "Connecting";
    private bool _isAllowedToSend;
    private bool _isConnected;
    private string _toggleSendingStateButton = "Start";
    private string? _errorMessage;
    private bool _errorOccurred;

    public MainWindow(IDeviceService deviceService)
    {
        _deviceService = deviceService;

        InitializeComponent();
        DataContext = this;

        Initialize();


        Closed += MainWindow_Closed;
    }

    public bool ErrorOccurred

    {
        get => _errorOccurred;
        private set => SetField(ref _errorOccurred, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public bool IsAllowedToSend
    {
        get => _isAllowedToSend;
        set
        {
            SetField(ref _isAllowedToSend, value);
        }
    }

    public string ConnectionStateMessage
    {
        get => _connectionStateMessage;
        set => SetField(ref _connectionStateMessage, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (value == _isConnected)
            {
                return;
            }

            SetField(ref _isConnected, value);
            OnPropertyChanged();
        }
    }

    public string ToggleSendingStateButton
    {
        get => _toggleSendingStateButton;
        set => SetField(ref _toggleSendingStateButton, value);
    }

    private void Initialize()
    {
        _deviceService.DeviceActionStateChangedEvent += OnSendingMessagesStateChangedEvent;
        _deviceService.DeviceConnectionStateChangedEvent += OnConnectionStateChangedEvent;
        _deviceService.DeviceServiceErrorEvent += OnDeviceServiceErrorEvent;

        _deviceService.ConnectDeviceAsync().ConfigureAwait(false);
    }


    private void ButtonToggleSendingState_OnClick(object sender, RoutedEventArgs e)
    {
        _deviceService.ChangeSendingAllowed(!IsAllowedToSend);

        Task.Run(async () =>
        {
            while (IsAllowedToSend)
            {
                await _deviceService.SendMessageAsync(new { running = _isAllowedToSend });
            }
        }).ConfigureAwait(false);
    }

    private void OnConnectionStateChangedEvent(object? sender,
        ConnectionStateArgs e)
    {
        IsConnected = e.IsConnected;
        ConnectionStateMessage = e.Message;
    }

    private void OnDeviceServiceErrorEvent(object? sender, string e)
    {
        ErrorMessage = e;

        ErrorOccurred = true;
    }

    private void OnSendingMessagesStateChangedEvent(object? sender,
        SendingMessagesArgs e)
    {
        IsAllowedToSend = e.IsAllowedToSend;

        Dispatcher.Invoke(() =>
        {
            ToggleSendingStateButton = IsAllowedToSend ? "Stop" : "Start";

            var sb = (BeginStoryboard)TryFindResource("SbRotate");
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

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _deviceService.DeviceActionStateChangedEvent -= OnSendingMessagesStateChangedEvent;
        _deviceService.DeviceConnectionStateChangedEvent -= OnConnectionStateChangedEvent;
        _deviceService.DeviceServiceErrorEvent -= OnDeviceServiceErrorEvent;

        Closed -= MainWindow_Closed;
    }
}