using System;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record GravitySettings
    {
        // backing fields
        [SerializeField]
        private double _defaultGravity = 0.02d;
        [SerializeField]
        private double _defaultLockDelay = .5d;
        [SerializeField]
        private double _piecePlacementDelay;
        [SerializeField]
        private double _lineClearDelay;
        [SerializeField]
        private double _hardLockAmount = 5d;

        [field: SerializeField]
        public LockDelayType LockDelayType { get; set; } = LockDelayType.OnIllegalMovement;
        [field: SerializeField]
        public HardLockType HardLockType { get; set; } = HardLockType.LimitedTime;
        [field: SerializeField]
        public TopoutCondition TopoutCondition { get; set; } = TopoutCondition.PieceSpawn;
        [field: SerializeField]
        public bool AllowClutchClears { get; set; } = true;

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

        public double HardLockAmount
        {
            get => _hardLockAmount;
            set => _hardLockAmount = Math.Max(value, 0);
        }
    }
}