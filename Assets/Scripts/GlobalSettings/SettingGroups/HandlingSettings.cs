
/************************************
HandlingSettings.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UStacker.GlobalSettings.Enums;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record HandlingSettings
    {
        // DAS related settings
        [SerializeField] private double _delayedAutoShift = .125d;
        [SerializeField] private double _automaticRepeatRate = .05d;
        [SerializeField] private double _dasCutDelay;
        [SerializeField] private bool _cancelDasDelayWithInput = true;
        [SerializeField] private DelayDasOn _delayDasOn = DelayDasOn.Nothing;

        [SerializeField]
        private SimultaneousDasBehavior _simultaneousDasBehavior = SimultaneousDasBehavior.CancelBothDirections;

        // soft drop related settings
        [SerializeField] private double _softDropFactor = 20d;
        [SerializeField] private double _softDropDelay;
        [SerializeField] private double _minSoftDropGravity;
        [SerializeField] private double _maxSoftDropGravity = double.PositiveInfinity;
        [SerializeField] private double _zeroGravitySoftDropBase = 0.02;

        // other settings
        [SerializeField] private double _doubleDropPreventionInterval;
        [SerializeField] private DiagonalLockBehavior _diagonalLockBehavior = DiagonalLockBehavior.Disabled;

        [SerializeField]
        private InitialActionsType _initialActionsType = InitialActionsType.RegisterKeypressDuringDelay;

        [SerializeField] private bool _useInitialRotations = true;
        [SerializeField] private bool _useInitialHold;

        [FormerlySerializedAs("_automaticPreSpawnRotation")] [SerializeField]
        private AutomaticInitialRotation _automaticInitialRotation = AutomaticInitialRotation.RotateClockwise;

        private bool _isDirty;

        [JsonIgnore]
        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                _isDirty = value;
                if (_isDirty)
                    Dirtied?.Invoke();
            }
        }

        public DelayDasOn DelayDasOn
        {
            get => _delayDasOn;
            set
            {
                _delayDasOn = value;
                IsDirty = true;
            }
        }

        public SimultaneousDasBehavior SimultaneousDasBehavior
        {
            get => _simultaneousDasBehavior;
            set
            {
                _simultaneousDasBehavior = value;
                IsDirty = true;
            }
        }

        public DiagonalLockBehavior DiagonalLockBehavior
        {
            get => _diagonalLockBehavior;
            set
            {
                _diagonalLockBehavior = value;
                IsDirty = true;
            }
        }

        public bool CancelDasDelayWithInput
        {
            get => _cancelDasDelayWithInput;
            set
            {
                _cancelDasDelayWithInput = value;
                IsDirty = true;
            }
        }

        public double DelayedAutoShift
        {
            get => _delayedAutoShift;
            set
            {
                _delayedAutoShift = Math.Max(value, 0);
                IsDirty = true;
            }
        }

        public double AutomaticRepeatRate
        {
            get => _automaticRepeatRate;
            set
            {
                _automaticRepeatRate = Math.Max(value, 0);
                IsDirty = true;
            }
        }

        public double SoftDropFactor
        {
            get => _softDropFactor;
            set
            {
                _softDropFactor = Math.Max(value, 1);
                IsDirty = true;
            }
        }

        public double SoftDropDelay
        {
            get => _softDropDelay;
            set
            {
                _softDropDelay = Math.Max(value, 0);
                IsDirty = true;
            }
        }

        public double MinSoftDropGravity
        {
            get => _minSoftDropGravity;
            set
            {
                _minSoftDropGravity = Math.Max(0, Math.Min(value, _maxSoftDropGravity));
                IsDirty = true;
            }
        }

        public double MaxSoftDropGravity
        {
            get => _maxSoftDropGravity;
            set
            {
                _maxSoftDropGravity = Math.Max(0, Math.Max(value, _minSoftDropGravity));
                IsDirty = true;
            }
        }

        public bool UseInitialRotations
        {
            get => _useInitialRotations;
            set
            {
                _useInitialRotations = value;
                IsDirty = true;
            }
        }

        public bool UseInitialHold
        {
            get => _useInitialHold;
            set
            {
                _useInitialHold = value;
                IsDirty = true;
            }
        }

        public InitialActionsType InitialActionsType
        {
            get => _initialActionsType;
            set
            {
                _initialActionsType = value;
                IsDirty = true;
            }
        }

        public double ZeroGravitySoftDropBase
        {
            get => _zeroGravitySoftDropBase;
            set
            {
                _zeroGravitySoftDropBase = Math.Max(value, 0.01);
                IsDirty = true;
            }
        }

        public double DasCutDelay
        {
            get => _dasCutDelay;
            set
            {
                _dasCutDelay = Math.Max(value, 0);
                IsDirty = true;
            }
        }

        public double DoubleDropPreventionInterval
        {
            get => _doubleDropPreventionInterval;
            set
            {
                _doubleDropPreventionInterval = Math.Max(value, 0);
                IsDirty = true;
            }
        }

        public AutomaticInitialRotation AutomaticInitialRotation
        {
            get => _automaticInitialRotation;
            set
            {
                _automaticInitialRotation = value;
                IsDirty = true;
            }
        }

        public event Action Dirtied;

        public void Undirty()
        {
            IsDirty = false;
        }

        public void Override(HandlingSettings other)
        {
            DelayedAutoShift = other.DelayedAutoShift;
            AutomaticRepeatRate = other.AutomaticRepeatRate;
            DasCutDelay = other.DasCutDelay;
            CancelDasDelayWithInput = other.CancelDasDelayWithInput;
            DelayDasOn = other.DelayDasOn;
            SimultaneousDasBehavior = other.SimultaneousDasBehavior;

            SoftDropFactor = other.SoftDropFactor;
            SoftDropDelay = other.SoftDropDelay;
            MinSoftDropGravity = other.MinSoftDropGravity;
            MaxSoftDropGravity = other.MaxSoftDropGravity;
            ZeroGravitySoftDropBase = other.ZeroGravitySoftDropBase;

            DoubleDropPreventionInterval = other.DoubleDropPreventionInterval;
            DiagonalLockBehavior = other.DiagonalLockBehavior;
            InitialActionsType = other.InitialActionsType;
            AutomaticInitialRotation = other.AutomaticInitialRotation;
        }
    }
}
/************************************
end HandlingSettings.cs
*************************************/
