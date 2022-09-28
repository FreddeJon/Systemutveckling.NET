using System;
using AdministrationApp.MVVM.ViewModels;

namespace AdministrationApp.MVVM.Models;

public class DeviceItem
{
    private bool _isChecked;
    public string DeviceId { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public string DeviceType { get; set; } = "Unknown";

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (Equals(_isChecked, value)) return;
            _isChecked = value;
            HandleToggleState?.Invoke(this,new ToggleSendingStateArgs(DeviceId, _isChecked));
        }
    }

    public string IconActive { get; set; } = "";
    public string IconInActive { get; set; } = "";
    public string StateActive { get; set; } = "";
    public string StateInActive { get; set; } = "";
    public EventHandler<ToggleSendingStateArgs>? HandleToggleState { get; set; }
}