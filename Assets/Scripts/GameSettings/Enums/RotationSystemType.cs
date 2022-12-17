using System.ComponentModel;
using UnityEngine;

namespace UStacker.GameSettings.Enums
{
    public enum RotationSystemType : byte
    {
        [Tooltip("Pieces will not attempt to kick")]
        None = 0,
        [Tooltip("A modern rotation system used in most stacker games")]
        SRS = 1,
        [Tooltip("Slightly altered version of SRS")]
        [Description("SRS+")]
        SRSPlus = 2,
        [Tooltip("Custom rotation system will be used")]
        Custom = 3
    }
}