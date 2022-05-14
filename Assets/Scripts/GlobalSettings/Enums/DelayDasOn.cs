using System;

namespace Blockstacker.GlobalSettings.Enums
{
    [Flags]
    public enum DelayDasOn : byte
    {
        Nothing = 0,
        Placement = 1,
        Rotation = 2
    }
}