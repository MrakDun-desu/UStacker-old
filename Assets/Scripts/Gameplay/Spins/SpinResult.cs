
/************************************
SpinResult.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine;

namespace UStacker.Gameplay.Spins
{
    [Serializable]
    public record SpinResult
    {
        public bool WasSpin;
        public bool WasSpinMini;
        public bool WasSpinRaw;
        public bool WasSpinMiniRaw;
        public Vector2Int Kick = Vector2Int.zero;

        public SpinResult(Vector2Int kick, bool wasSpin = false, bool wasSpinMini = false)
        {
            Kick = kick;
            WasSpin = wasSpin;
            WasSpinMini = wasSpinMini;
        }

        public SpinResult()
        {
        }
    }
}
/************************************
end SpinResult.cs
*************************************/
