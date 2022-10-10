using System;

namespace AdministrationApp.Helpers;

public static class EnumHelper
{
    public static string GetRoom(this Room room)
    {
        return room switch
        {
            Room.Kitchen => "kitchen",
            Room.Bedroom => "bedroom",
            Room.Livingroom => "livingroom",
            Room.Unset => "Unset",
            _ => throw new ArgumentOutOfRangeException(nameof(room), room, null)
        };
    }
}