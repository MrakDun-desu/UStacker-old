using System;
using Blockstacker.Common.Attributes;

namespace Blockstacker.GlobalSettings.Enums
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