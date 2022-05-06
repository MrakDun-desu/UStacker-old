using System;
using Blockstacker.GlobalSettings.Enums;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public class HandlingSettings
    {
        public double DelayedAutoShift = .125f;
        public double AutomaticRepeatRate = 0;
        public double SoftDropFactor = float.PositiveInfinity;
        public double DasCutDelay = 0;
        public AntiDasBehavior AntiDasBehavior = AntiDasBehavior.CancelFirstDirection;
        public DiagonalLockBehavior DiagonalLockBehavior = DiagonalLockBehavior.DontLock;
    }
}
