using System;
using UnityEngine;

namespace Blockstacker.GameSettings.Enums
{
    [Flags]
    public enum GarbageGeneration : short
    {
        [Tooltip("No garbage will be generated")]
        None = 0,
        [Tooltip("Garbage will be generated with single block holes")]
        Singles = 1,
        [Tooltip("Garbage will be generated with double block holes")]
        Doubles = 1 << 1,
        [Tooltip("Garbage will be generated with triple block holes")]
        Triples = 1 << 2,
        [Tooltip("Garbage will be generated with quad block holes")]
        Quads = 1 << 3
    }
}