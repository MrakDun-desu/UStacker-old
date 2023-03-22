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
            writer.WriteDouble(handling.SoftDropFactor);
            
            writer.WriteDouble(handling.DasCutDelay);
            writer.WriteDouble(handling.DoubleDropPreventionInterval);
            writer.WriteDouble(handling.SoftDropDelay);
            writer.WriteDouble(handling.ZeroGravitySoftDropBase);
            
            writer.WriteByte((byte)handling.DelayDasOn);
            writer.WriteByte((byte)handling.DiagonalLockBehavior);
            writer.WriteByte((byte)handling.SimultaneousDasBehavior);
            
            writer.WriteBoolean(handling.CancelDasDelayWithInput);
        }

        [UsedImplicitly]
        public static HandlingSettings ReadHandling(this Reader reader)
        {
            return new HandlingSettings
            {
                DelayedAutoShift = reader.ReadDouble(),
                AutomaticRepeatRate = reader.ReadDouble(),
                SoftDropFactor = reader.ReadDouble(),
                
                DasCutDelay = reader.ReadDouble(),
                DoubleDropPreventionInterval = reader.ReadDouble(),
                SoftDropDelay = reader.ReadDouble(),
                ZeroGravitySoftDropBase = reader.ReadDouble(),
                
                DelayDasOn = (DelayDasOn)reader.ReadByte(),
                DiagonalLockBehavior = (DiagonalLockBehavior)reader.ReadByte(),
                SimultaneousDasBehavior = (SimultaneousDasBehavior)reader.ReadByte(),
                
                CancelDasDelayWithInput = reader.ReadBoolean()
            };
        }
    }
    
    [Serializable]
    public record HandlingSettings
    {
        [SerializeField] private double _delayedAutoShift = .125d;
        [SerializeField] private double _automaticRepeatRate = .05d;
        [SerializeField] private double _softDropFactor = 20d;
        [SerializeField] private double _dasCutDelay;
        [SerializeField] private double _doubleDropPreventionInterval;
        [SerializeField] private double _softDropDelay;
        [SerializeField] private DelayDasOn _delayDasOn = DelayDasOn.Nothing;
        [SerializeField] private DiagonalLockBehavior _diagonalLockBehavior = DiagonalLockBehavior.DontLock;
        [SerializeField] private bool _cancelDasDelayWithInput = true;
        [SerializeField] private double _zeroGravitySoftDropBase = 0.02;

        [SerializeField]
        private SimultaneousDasBehavior _simultaneousDasBehavior = SimultaneousDasBehavior.CancelFirstDirection;

        [JsonIgnore] private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                _isDirty = value;
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
            DelayedAutoShift = other._delayedAutoShift;
            AutomaticRepeatRate = other._automaticRepeatRate;
            SoftDropFactor = other._softDropFactor;
            DasCutDelay = other._dasCutDelay;
            DoubleDropPreventionInterval = other._doubleDropPreventionInterval;
            SoftDropDelay = other._softDropDelay;
            DelayDasOn = other._delayDasOn;
            DiagonalLockBehavior = other._diagonalLockBehavior;
            CancelDasDelayWithInput = other._cancelDasDelayWithInput;
            ZeroGravitySoftDropBase = other._zeroGravitySoftDropBase;
            SimultaneousDasBehavior = other._simultaneousDasBehavior;
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
    }
}