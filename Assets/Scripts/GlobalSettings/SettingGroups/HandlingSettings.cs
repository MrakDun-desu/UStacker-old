using System;
using Blockstacker.GlobalSettings.Enums;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record HandlingSettings
    {
        public double DelayedAutoShift = .125d;
        public double AutomaticRepeatRate = .0d;
        public double SoftDropFactor = double.PositiveInfinity;
        public double DasCutDelay = 0;
        public double DoubleDropPreventionInterval = 0;
        public DelayDasOn DelayDasOn = DelayDasOn.Nothing;
        public AntiDasBehavior AntiDasBehavior = AntiDasBehavior.CancelFirstDirection;
        public DiagonalLockBehavior DiagonalLockBehavior = DiagonalLockBehavior.DontLock;
    }
}
