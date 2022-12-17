using UnityEngine;

namespace UStacker.GameSettings.Enums
{
    public enum LockDelayType : byte
    {
        [Tooltip("Lock delay and hard lock will start when piece touches ground")]
        OnTouchGround,
        [Tooltip("Lock delay and hard lock will start when piece tries to move down when it can't")]
        OnIllegalMovement
    }
}