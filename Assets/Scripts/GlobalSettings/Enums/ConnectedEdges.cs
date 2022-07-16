using System;

namespace Blockstacker.GlobalSettings.Enums
{
    [Flags]
    public enum ConnectedEdges : byte
    {
        None =        0b00000000,
        Top =         0b00000001,
        Left =        0b00000010,
        Right =       0b00000100,
        Bottom =      0b00001000,
        TopLeft =     0b00010000,
        TopRight =    0b00100000,
        BottomLeft =  0b01000000,
        BottomRight = 0b10000000
    }
}