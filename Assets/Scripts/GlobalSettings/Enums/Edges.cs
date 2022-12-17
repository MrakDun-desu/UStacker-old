using System;

namespace UStacker.GlobalSettings.Enums
{
    [Flags]
    public enum Edges : byte
    {
        None = 0b00000000, // 0
        Top = 0b00000001, // 1
        Left = 0b00000010, // 2
        Right = 0b00000100, // 4
        Bottom = 0b00001000, // 8
        TopLeft = 0b00010000, // 16
        TopRight = 0b00100000, // 32
        BottomLeft = 0b01000000, // 64
        BottomRight = 0b10000000 // 128
    }
}