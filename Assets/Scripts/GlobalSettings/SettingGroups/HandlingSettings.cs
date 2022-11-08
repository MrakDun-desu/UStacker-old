using System;
using Blockstacker.GlobalSettings.Enums;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record HandlingSettings
    {
        // backing fields 
        private double _delayedAutoShift = .125d;
        private double _automaticRepeatRate = .2d;
        private double _softDropFactor = 20d;
        private double _dasCutDelay = 0d;
        private double _doubleDropPreventionInterval = 0;

        public double DelayedAutoShift
        {
            get => _delayedAutoShift;
            set => _delayedAutoShift = Math.Max(value, 0);
        }

        public double AutomaticRepeatRate
        {
            get => _automaticRepeatRate;
            set => _automaticRepeatRate = Math.Max(value, 0);
        }

        public double SoftDropFactor
        {
            get => _softDropFactor;
            set => _softDropFactor = Math.Max(value, 1);
        }

        public double DasCutDelay
        {
            get => _dasCutDelay;
            set => _dasCutDelay = Math.Max(value, 0);
        }

        public double DoubleDropPreventionInterval
        {
            get => _doubleDropPreventionInterval;
            set => _doubleDropPreventionInterval = Math.Max(value, 0);
        }

        public DelayDasOn DelayDasOn { get; set; } = DelayDasOn.Nothing;

        public SimultaneousDasBehavior SimultaneousDasBehavior { get; set; } = SimultaneousDasBehavior.CancelFirstDirection;

        public DiagonalLockBehavior DiagonalLockBehavior { get; set; } = DiagonalLockBehavior.DontLock;

        public bool CancelDelayWithMovement { get; set; } = true;

    }
}