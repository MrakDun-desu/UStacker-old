using System;

namespace Blockstacker.GameSettings.Enums
{
    [Flags]
    public enum GarbageGeneration : short
    {
        None = 0,
        Singles = 1,
        Doubles = 1 << 1,
        Triples = 1 << 2,
        Quads = 1 << 3,
        CustomFlag = 1 << 4
    }
}