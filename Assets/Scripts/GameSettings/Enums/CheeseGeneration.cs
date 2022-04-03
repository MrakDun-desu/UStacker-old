using System;

namespace Blockstacker.GameSettings.Enums
{
    [Flags]
    public enum CheeseGeneration
    {
        None = 0,
        Singles = 1,
        Doubles = 2,
        Triples = 4,
        Quads = 8
    }
}