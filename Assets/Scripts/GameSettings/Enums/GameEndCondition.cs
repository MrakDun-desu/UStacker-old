using UnityEngine;

namespace UStacker.GameSettings.Enums
{
    public enum GameEndCondition : byte
    {
        [Tooltip("Game will continue until the player tops out")]
        None = 0,

        [Tooltip("Game will end when certain number of lines is cleared")]
        LinesCleared = 1,

        [Tooltip("Game will end when certain number of garbage lines is cleared")]
        GarbageLinesCleared = 2,

        [Tooltip("Game will end when certain number of pieces is placed")]
        PiecesPlaced = 3,

        [Tooltip("Game will end when certain score is reached")]
        Score = 4,

        [Tooltip("Game will end when certain time has passed")]
        Time = 5
    }
}