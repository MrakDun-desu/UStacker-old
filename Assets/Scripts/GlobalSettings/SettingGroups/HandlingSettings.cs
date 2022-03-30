using System;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public class HandlingSettings
    {
        public float DelayedAutoShift = .125f;
        public float AutomaticRepeatRate = 0;
        public float SoftDropFactor = float.PositiveInfinity;
        public float DasCutDelay = 0;
        public bool UseDasCancelling = true;
        public bool UseDiagonalLock = false;

    }
}
