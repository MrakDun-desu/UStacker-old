using System;
using UStacker.Common.Attributes;

namespace UStacker.GlobalSettings.Enums
{
    [Flags]
    public enum DelayDasOn : byte
    {
        [IgnoreInUI]
        Nothing = 0,
        Placement = 1,
        Rotation = 2
    }
}