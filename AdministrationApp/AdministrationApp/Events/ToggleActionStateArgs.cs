using System;

namespace AdministrationApp.Events;

public class ToggleActionStateArgs : EventArgs
{
    public bool State { get; }
    public string DeviceId { get; }

    public ToggleActionStateArgs(bool state, string deviceId)
    {
        State = state;
        DeviceId = deviceId;
    }
}