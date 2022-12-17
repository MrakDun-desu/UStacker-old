using UnityEngine;

namespace UStacker.GameSettings.Enums
{
    public enum HardLockType : byte
    {
        [Tooltip("Piece will hard lock after limited amount of time")]
        LimitedTime,
        [Tooltip("Piece will hard lock after limited amount of movements or rotations")]
        LimitedMoves,
        [Tooltip("Piece will hard lock after limited amount of inputs")]
        LimitedInputs,
        [Tooltip("Piece will never hard lock")]
        InfiniteMovement
    }
}