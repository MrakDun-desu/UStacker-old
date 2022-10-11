using System.ComponentModel;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Enums
{
    public enum DiagonalLockBehavior : byte
    {
        [Tooltip("Process both directions")]
        [Description("Don't Lock")]
        DontLock = 0,
        [Tooltip("Only process vertical movement")]
        PrioritizeVertical = 1,
        [Tooltip("Only process horizontal movement")]
        PrioritizeHorizontal = 2
    }
}