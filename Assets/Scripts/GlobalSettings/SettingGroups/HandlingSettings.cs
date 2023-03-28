using System;
using FishNet.Serializing;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UStacker.GlobalSettings.Enums;
using UnityEngine;

namespace UStacker.GlobalSettings.Groups
{
    [UsedImplicitly]
    public static class HandlingSerializer
    {
        [UsedImplicitly]
        public static void WriteHandling(this Writer writer, HandlingSettings handling)
        {
            writer.WriteDouble(handling.DelayedAutoShift);
            writer.WriteDouble(handling.AutomaticRepeatRate);
            writer.WriteDouble(handling.DasCutDelay);
            writer.WriteBoolean(handling.CancelDasDelayWithInput);
            writer.WriteByte((byte) handling.DelayDasOn);
            writer.WriteByte((byte) handling.SimultaneousDasBehavior);

            writer.WriteDouble(handling.SoftDropFactor);
            writer.WriteDouble(handling.SoftDropDelay);
            writer.WriteDouble(handling.MinSoftDropGravity);
            writer.WriteDouble(handling.MaxSoftDropGravity);
            writer.WriteDouble(handling.ZeroGravitySoftDropBase);

            writer.WriteDouble(handling.DoubleDropPreventionInterval);
            writer.WriteByte((byte) handling.DiagonalLockBehavior);
            writer.WriteByte((byte) handling.InputBufferingType);
            writer.WriteByte((byte) handling.AutomaticPreSpawnRotation);
        }

        [UsedImplicitly]
        public static HandlingSettings ReadHandling(this Reader reader)
        {
            return new HandlingSettings
            {
                DelayedAutoShift = reader.ReadDouble(),
                AutomaticRepeatRate = reader.ReadDouble(),
                DasCutDelay = reader.ReadDouble(),
                CancelDasDelayWithInput = reader.ReadBoolean(),
                DelayDasOn = (DelayDasOn) reader.ReadByte(),
                SimultaneousDasBehavior = (SimultaneousDasBehavior) reader.ReadByte(),

                SoftDropFactor = reader.ReadDouble(),
                SoftDropDelay = reader.ReadDouble(),
                MinSoftDropGravity = reader.ReadDouble(),
                MaxSoftDropGravity = reader.ReadDouble(),
                ZeroGravitySoftDropBase = reader.ReadDouble(),
                
                DoubleDropPreventionInterval = reader.ReadDouble(),
                DiagonalLockBehavior = (DiagonalLockBehavior) reader.ReadByte(),
                InputBufferingType = (InputBufferingType) reader.ReadByte(),
                AutomaticPreSpawnRotation = (AutomaticPreSpawnRotation) reader.ReadByte()
            };
        }
    }

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
        [SerializeField] private DiagonalLockBehavior _diagonalLockBehavior = DiagonalLockBehavior.DontLock;
        [SerializeField] private InputBufferingType _inputBufferingType = InputBufferingType.DontBuffer;
        [SerializeField]
        private AutomaticPreSpawnRotation _automaticPreSpawnRotation = AutomaticPreSpawnRotation.DontRotate;

        [JsonIgnore] private bool _isDirty;

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
            InputBufferingType = other.InputBufferingType;
            AutomaticPreSpawnRotation = other.AutomaticPreSpawnRotation;
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

        public InputBufferingType InputBufferingType
        {
            get => _inputBufferingType;
            set
            {
                _inputBufferingType = value;
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
        
        public AutomaticPreSpawnRotation AutomaticPreSpawnRotation
        {
            get => _automaticPreSpawnRotation;
            set
            {
                _automaticPreSpawnRotation = value;
                IsDirty = true;
            }
        }
    }
}