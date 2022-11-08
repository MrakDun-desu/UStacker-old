using System;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record GravitySettings
    {
        // backing fields
        private double _defaultGravity = 0.02d;
        private double _defaultLockDelay = .5d;
        private double _piecePlacementDelay = 0;
        private double _lineClearDelay = 0;
        private double _hardLockAmount = 0;

        public double DefaultGravity
        {
            get => _defaultGravity;
            set => _defaultGravity = Math.Max(value, 0);
        }

        public double DefaultLockDelay
        {
            get => _defaultLockDelay;
            set => _defaultLockDelay = Math.Max(value, 0);
        }

        public LockDelayType LockDelayType { get; set; } = LockDelayType.OnIllegalMovement;

        public double PiecePlacementDelay
        {
            get => _piecePlacementDelay;
            set => _piecePlacementDelay = Math.Max(value, 0);
        }

        public double LineClearDelay
        {
            get => _lineClearDelay;
            set => _lineClearDelay = Math.Max(value, 0);
        }

        public HardLockType HardLockType { get; set; } = HardLockType.LimitedTime;

        public double HardLockAmount
        {
            get => _hardLockAmount;
            set => _hardLockAmount = Math.Max(value, 0);
        }

        public TopoutCondition TopoutCondition { get; set; } = TopoutCondition.AllBlocksAboveLethal;

        public bool AllowClutchClears { get; set; } = true;
    }
}