using System;
using AdministrationApp.Events;

namespace AdministrationApp.MVVM.Models;

public class DeviceItem
{
    private bool _actionState;
    public string DeviceId { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public string DeviceType { get; set; } = "Unknown";

    public bool ActionState
    {
        get => _actionState;
        set
        {
            if (Equals(_actionState, value)) return;
            _actionState = value;
            ToggleActionStateEvent?.Invoke(this,new ToggleActionStateArgs(_actionState, DeviceId));
        }
    }

    public string IconActiveState { get; set; } = "";
    public string IconInActiveState { get; set; } = "";
    public string TextActiveState { get; set; } = "";
    public string TextInActiveState { get; set; } = "";
    public EventHandler<ToggleActionStateArgs> ToggleActionStateEvent { get; set; }
}