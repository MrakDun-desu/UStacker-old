using System;
using Blockstacker.GlobalSettings.Enums;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public class HandlingSettings
    {
        public float DelayedAutoShift = .125f;
        public float AutomaticRepeatRate = 0;
        public float SoftDropFactor = float.PositiveInfinity;
        public float DasCutDelay = 0;
        public AntiDasBehavior AntiDasBehavior = AntiDasBehavior.CancelFirstDirection;
        public DiagonalLockBehavior DiagonalLockBehavior = DiagonalLockBehavior.DontLock;
    }
}
