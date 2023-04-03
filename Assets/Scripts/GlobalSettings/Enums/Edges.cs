using System;

namespace UStacker.GlobalSettings.Enums
{
    [Flags]
    public enum Edges : byte
    {
        None = 0, // 0
        Top = 1, // 1
        Left = 1 << 1, // 2
        Right = 1 << 2, // 4
        Bottom = 1 << 3, // 8
        TopLeft = 1 << 4, // 16
        TopRight = 1 << 5, // 32
        BottomLeft = 1 << 6, // 64
        BottomRight = 1 << 7 // 128
    }
}