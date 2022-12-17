using UnityEngine;

namespace UStacker.GameSettings.Enums
{
    public enum GameManagerType
    {
        [Tooltip("No levelling, no score, gravity and lock delay stay at default")]
        None = 0,
        [Tooltip("Will detect spins, perfect clears, combos. Gravity and lock delay stay at default")]
        ModernWithoutLevelling = 1,
        [Tooltip("Will detect spins, perfect clears, combos. Gravity and lock delay change with the level")]
        ModernWithLevelling = 2,
        [Tooltip("Will compute score only depending on level and lines cleared. Will change gravity depending on level. Lock delay is 0")]
        Classic = 3,
        [Tooltip("Pick this option for custom game manager")]
        Custom = 4
    }
}