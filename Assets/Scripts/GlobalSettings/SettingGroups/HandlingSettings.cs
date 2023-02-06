using System;
using UStacker.GlobalSettings.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record HandlingSettings
    {
        // backing fields 
        [SerializeField]
        private double _delayedAutoShift = .125d;
        [SerializeField]
        private double _automaticRepeatRate = .05d;
        [SerializeField]
        private double _softDropFactor = 20d;
        [SerializeField]
        private double _dasCutDelay;
        [SerializeField]
        private double _doubleDropPreventionInterval;
        [SerializeField]
        private double _softDropDelay;
        [FormerlySerializedAs("_softDropZeroGravityReplacement")] [SerializeField] 
        private double _zeroGravitySoftDropBase = 0.02;

        [field: SerializeField]
        public DelayDasOn DelayDasOn { get; set; } = DelayDasOn.Nothing;
        [field: SerializeField]
        public SimultaneousDasBehavior SimultaneousDasBehavior { get; set; } = SimultaneousDasBehavior.CancelFirstDirection;
        [field: SerializeField]
        public DiagonalLockBehavior DiagonalLockBehavior { get; set; } = DiagonalLockBehavior.DontLock;
        [field: SerializeField]
        public bool CancelDasDelayWithInput { get; set; } = true;

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

        public double SoftDropDelay
        {
            get => _softDropDelay;
            set => _softDropDelay = Math.Max(value, 0);
        }

        public double ZeroGravitySoftDropBase
        {
            get => _zeroGravitySoftDropBase;
            set => _zeroGravitySoftDropBase = Math.Max(value, 0.01);
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
    }
}